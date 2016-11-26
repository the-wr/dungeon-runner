using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class Mechanics
{
    private GameState gameState;
    private PhaseManager phaseManager;
    private RoomManager roomManager;
    private HeroManager heroManager;
    private EffectManager effectManager;

    public RoomManager RoomManager { get { return roomManager; } }
    public EffectManager EffectManager { get { return effectManager; } }

    public void StartGame( GameState gameState, PhaseManager phaseManager )
    {
        this.gameState = gameState;
        this.phaseManager = phaseManager;

        phaseManager.NewTurn += OnNewTurn;
        phaseManager.TurnEnding += OnTurnEnding;
        phaseManager.GameStarting += OnGameStarting;

        LoadDecks();

        for ( int i = 0; i < Constants.StartHandSize; ++i )
        {
            DrawCardIfPossible( gameState.HandOverlord, gameState.DeckOverlord, CardType.Room );
            DrawCardIfPossible( gameState.HandRunner, gameState.DeckRunner, CardType.Hero );
        }

        gameState.MaxManaRunner = Constants.StartManaRunner;
        gameState.MaxManaOverlord = Constants.StartManaOverlord;

        roomManager = new RoomManager();
        roomManager.Init();

        heroManager = new HeroManager();
        heroManager.Init();

        effectManager = new EffectManager();
        effectManager.Init( roomManager );
    }

    public void PlayCard( GameObject player, string cardId, GameObject targetSlot )
    {
        CardDesc card;
        if ( !CardDB.Instance.Cards.TryGetValue( cardId, out card ) )
            return;

        if ( player == gameState.PlayerOverlord )
        {
            if ( !phaseManager.IsOverlordTurn )
                return;

            if ( !gameState.HandOverlord.Contains( cardId ) )
                return;

            if ( card.GetManaToCast() > gameState.ManaOverlord )
                return;

            PlayOverlordCardInternal( card, targetSlot );
        }
        else if ( player == gameState.PlayerRunner )
        {
            if ( !phaseManager.IsRunnerTurn )
                return;

            if ( !gameState.HandRunner.Contains( cardId ) )
                return;

            if ( card.GetManaToCast() > gameState.ManaRunner )
                return;

            PlayRunnerCardInternal( card );
        }
    }

    public void ActivateRoom( GameObject player, GameObject room )
    {
        if ( !phaseManager.IsOverlordTurn )
            return;

        var ri = room.GetComponent<RoomInfo>();

        CardDesc cardDesc;
        if ( !CardDB.Instance.Cards.TryGetValue( ri.CardId, out cardDesc ) )
            return;

        if ( gameState.ManaOverlord < cardDesc.ManaCost )
            return;

        gameState.ManaOverlord -= cardDesc.ManaCost;
        ri.IsActive = true;

        if ( cardDesc.Type == CardType.Enchantment && !cardDesc.IsAmbush )
            roomManager.RevealRoom( room, ri, true );
    }

    public void StartRun( GameObject player, GameObject roomSlot )
    {
        if ( gameState.RunState != null )
            return;

        int wing = roomSlot.GetComponent<RoomSlotInfo>().Wing;
        var firstRoom = roomManager.GetFirstRoom( wing );
        if ( firstRoom == null )
            return;

        var runStateObj = GameObject.Instantiate( GameRoot.Instance.RunStatePrefab );
        var runState = runStateObj.GetComponent<RunState>();

        runState.Room = firstRoom;
        runState.Stage = RunStage.Approach;

        NetworkServer.Spawn( runStateObj );

        gameState.RunState = runStateObj;
    }

    public void EnterRoom()
    {
        if ( gameState.RunState == null )
            return;

        var runState = gameState.RunState.GetComponent<RunState>();
        if ( runState.Stage != RunStage.Approach )
            return;

        runState.Stage = RunStage.Combat;

        var ri = runState.Room.GetComponent<RoomInfo>();
        if ( ri.IsActive && !ri.IsRevealed )
            roomManager.RevealRoom( runState.Room, ri, true );

        if ( CardDB.Instance.Cards[ri.CardId].Type == CardType.Heart )
        {
            var enchantment = roomManager.GetEnchantmentInWing( ri.Slot.GetComponent<RoomSlotInfo>().Wing );
            if ( enchantment != null )
            {
                var ei = enchantment.GetComponent<RoomInfo>();
                if ( ei.IsActive && !ei.IsRevealed )
                    roomManager.RevealRoom( enchantment, ei, true );
            }
        }

        CheckRoomHealthAndAdvance();
    }

    public void Retreat()
    {
        if ( gameState.RunState == null )
            return;

        EndRun();

        phaseManager.EndTurn();
    }

    public void AttackWithHero( GameObject hero )
    {
        if ( gameState.RunState == null )
            return;

        var runState = gameState.RunState.GetComponent<RunState>();
        if ( runState.Room == null )
            return;

        var hi = hero.GetComponent<HeroInfo>();
        var ri = runState.Room.GetComponent<RoomInfo>();

        if( hi == null || ri == null )
            return;

        if ( hi.Damage > 0 )
            ri.DamageDealt += hi.Damage;
        if ( ri.Damage > 0 )
            hi.Health -= ri.Damage;

        if ( hi.Health <= 0 )
        {
            heroManager.RemoveHero( hero );
            GameObject.Destroy( hero );
        }
        else if ( ri.IsBroken )
        {
            hi.Frags++;
        }

        CheckRoomHealthAndAdvance();
        CheckHeroes();
    }

    public void MulliganReplaceCard( GameObject gameObject, string cardId )
    {
        if ( gameObject == gameState.PlayerOverlord && !phaseManager.MulliganOverlordFinished )
            gameState.HandOverlord.Remove( cardId );

        if ( gameObject == gameState.PlayerRunner && !phaseManager.MulliganRunnerFinished )
            gameState.HandRunner.Remove( cardId );
    }

    // -----

    private void LoadDecks()
    {
        // TODO - Check validity (card existence)

        gameState.DeckOverlord =
            XmlSerializeHelper.LoadFromXml<List<string>>( Path.Combine( Constants.DataFolder,
                "Decks\\DeckOverlord.xml" ) );

        gameState.DeckRunner =
            XmlSerializeHelper.LoadFromXml<List<string>>( Path.Combine( Constants.DataFolder,
                "Decks\\DeckRunner.xml" ) );

        gameState.DeckOverlord.Shuffle();
        gameState.DeckRunner.Shuffle();
    }

    private void OnNewTurn( GameObject obj )
    {
        bool isRunner = obj == gameState.PlayerRunner;

        if ( isRunner )
        {
            gameState.RoundNumber++;

            if ( gameState.MaxManaRunner < Constants.MaxMana )
                gameState.MaxManaRunner++;

            gameState.ManaRunner = gameState.MaxManaRunner;

            heroManager.PrepareHeroes();
        }
        else
        {
            if ( gameState.MaxManaOverlord < Constants.MaxMana )
                gameState.MaxManaOverlord++;

            gameState.ManaOverlord = gameState.MaxManaOverlord;

            roomManager.RestoreRooms();
        }
    }

    private void OnTurnEnding( GameObject obj )
    {
        EndRun();

        bool isRunner = obj == gameState.PlayerRunner;
        if ( isRunner )
        {
            for ( int i = 0; i < Constants.CardsPerTurn; ++i )
                DrawCardIfPossible( gameState.HandRunner, gameState.DeckRunner );

            if ( gameState.RoundNumber > 1 )
            {
                var hearts = roomManager.GetHearts();
                foreach ( var heart in hearts )
                {
                    var ri = heart.GetComponent<RoomInfo>();
                    if ( ri.IsBroken )
                        gameState.OverlordLife--;
                    else
                        gameState.OverlordVictorySteps--;
                }
            }
        }
        else
        {
            for ( int i = 0; i < Constants.CardsPerTurn; ++i )
                DrawCardIfPossible( gameState.HandOverlord, gameState.DeckOverlord );
        }

        CheckGameEnd();
    }

    private void OnGameStarting()
    {
        var toDraw = Constants.StartHandSize - gameState.HandRunner.Count;
        for ( int i = 0; i < toDraw; ++i )
            DrawCardIfPossible( gameState.HandRunner, gameState.DeckRunner, CardType.Hero );

        toDraw = Constants.StartHandSize - gameState.HandOverlord.Count;
        for ( int i = 0; i < toDraw; ++i )
            DrawCardIfPossible( gameState.HandOverlord, gameState.DeckOverlord, CardType.Room );

        gameState.DeckRunner.Shuffle();
        gameState.DeckOverlord.Shuffle();

        for ( int i = 0; i < Constants.CardsPerTurn; ++i )
        {
            DrawCardIfPossible( gameState.HandOverlord, gameState.DeckOverlord, CardType.Room );
            DrawCardIfPossible( gameState.HandRunner, gameState.DeckRunner );
        }
    }

    private void DrawCardIfPossible( SyncListString hand, List<string> deck, CardType? specificType = null )
    {
        if ( deck.Count == 0 )
            return;

        if ( hand.Count >= Constants.MaxHandSize )
            return;

        for ( int i = 0; i < deck.Count; ++i )
        {
            var card = deck[i];

            CardDesc cardDesc;
            if ( !CardDB.Instance.Cards.TryGetValue( card, out cardDesc ) )
                continue;

            if ( specificType == null || cardDesc.Type == specificType.Value )
            {
                deck.RemoveAt( i );
                hand.Add( card );
                return;
            }
        }
    }

    private void PlayOverlordCardInternal( CardDesc card, GameObject targetSlot )
    {
        if ( card.Type == CardType.Room )
        {
            if ( !roomManager.TryCastRoomInSlot( card, targetSlot ) )
                return;
        }

        if ( card.Type == CardType.Enchantment )
        {
            if ( !roomManager.TryCastEnchantmentInSlot( card, targetSlot ) )
                return;
        }

        gameState.HandOverlord.Remove( card.Id );
        gameState.ManaOverlord -= card.GetManaToCast();
    }

    private void PlayRunnerCardInternal( CardDesc card )
    {
        if ( card.Type == CardType.Hero )
        {
            if ( !heroManager.TrySummonHero( card ) )
                return;
        }

        gameState.HandRunner.Remove( card.Id );
        gameState.ManaRunner -= card.GetManaToCast();
    }

    private void CheckRoomHealthAndAdvance()
    {
        if ( gameState.RunState == null )
            return;

        var runState = gameState.RunState.GetComponent<RunState>();
        var ri = runState.Room.GetComponent<RoomInfo>();

        CardDesc cardDesc;
        if ( !CardDB.Instance.Cards.TryGetValue( ri.CardId, out cardDesc ) )
            return;

        if ( ri.IsBroken || !ri.IsActive )
        {
            if ( cardDesc.Type == CardType.Heart )
            {
                var ench = roomManager.GetEnchantmentInWing( ri.Slot.GetComponent<RoomSlotInfo>().Wing );
                if ( ench != null )
                {
                    var ei = ench.GetComponent<RoomInfo>();
                    ei.IsBroken = true;
                    roomManager.RevealRoom( runState.Room, ei, false );
                }

                EndRun();
                return;
            }

            var nextRoom = roomManager.GetNextRoom( runState.Room );
            if ( nextRoom == null )
            {
                EndRun();
                return;
            }

            runState.Room = nextRoom;
            runState.Stage = RunStage.Approach;
        }
    }

    private void CheckHeroes()
    {
        if ( !heroManager.CanAttack() )
            EndRun();
    }

    private void EndRun()
    {
        var obj = gameState.RunState;
        gameState.RunState = null;

        if ( obj != null )
            GameObject.Destroy( obj );
    }

    private void CheckGameEnd()
    {
        if ( gameState.OverlordLife <= 0 || gameState.OverlordVictorySteps <= 0 )
        {
            var announcer = GameObject.Instantiate( GameRoot.Instance.VictoryAnnouncerPrefab );
            var info = announcer.GetComponent<VictoryAnnouncer>();

            if ( gameState.OverlordLife <= 0 && gameState.OverlordVictorySteps <= 0 )
                info.Victor = "Nobody";
            else if ( gameState.OverlordVictorySteps <= 0 )
                info.Victor = "Overlord";
            else
                info.Victor = "Runner";

            NetworkServer.Spawn( announcer );
        }
    }
}
