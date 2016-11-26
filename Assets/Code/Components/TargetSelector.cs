using System;
using UnityEngine;
using System.Collections;

public class TargetSelector: MonoBehaviour
{
    public static TargetSelector Instance { get { return Helpers.LocalPlayer.GetComponent<TargetSelector>(); } }

    private CardDesc card;
    private bool isRunStarting;

    public bool IsActive { get { return card != null || isRunStarting; } }

    public void SelectTargetAndCastSpell( CardDesc cardDesc )
    {
        if ( GameState.Instance.IsMulliganState )
        {
            PlayerCommands.Instance.CmdMulliganReplaceCard( cardDesc.Id );
            return;
        }

        if ( IsActive )
            return;

        if ( !CanCast( cardDesc ) )
            return;

        if ( cardDesc.Type == CardType.Room )
        {
            card = cardDesc;

            var roomSlotsRenderer = GetComponent<RoomSlotsRenderer>();
            roomSlotsRenderer.HighlightForNewRoom();
        }

        if ( cardDesc.Type == CardType.Enchantment )
        {
            card = cardDesc;

            var roomSlotsRenderer = GetComponent<RoomSlotsRenderer>();
            roomSlotsRenderer.HighlightForNewEnchantment();
        }

        if ( cardDesc.Type == CardType.Hero )
        {
            PlayerCommands.Instance.CmdPlayCard( cardDesc.Id, null );
        }
    }

    public void SelectWingAndStartRun()
    {
        if ( IsActive )
            return;

        if ( !Helpers.IsRunnerTurn() )
            return;

        isRunStarting = true;

        var roomSlotsRenderer = GetComponent<RoomSlotsRenderer>();
        roomSlotsRenderer.HightlightForAttack();
    }

    // -----

    // TODO: Crap
    public void OnTargetSelected( GameObject target )
    {
        if ( Helpers.IsLocalPlayerRunner() &&
             Helpers.IsRunnerTurn() &&
             GameState.Instance.RunState != null &&
             GameState.Instance.RunState.GetComponent<RunState>().Stage == RunStage.Combat )
        {
            PlayerCommands.Instance.CmdAttackWithHero( target );
            return;
        }

        if ( !IsActive )
            return;

        if ( card != null )
            PlayerCommands.Instance.CmdPlayCard( card.Id, target );

        if ( isRunStarting )
            PlayerCommands.Instance.CmdStartRun( target );

        card = null;
        isRunStarting = false;

        UnhighlightAll();
    }

    // -----

    private void Update()
    {
        if ( Input.GetKey( KeyCode.Escape ) )
        {
            UnhighlightAll();

            card = null;
            isRunStarting = false;
        }
    }

    private void UnhighlightAll()
    {
        var roomSlotsRenderer = GetComponent<RoomSlotsRenderer>();
        roomSlotsRenderer.UnhighlightAll();

    }

    private bool CanCast( CardDesc cardDesc )
    {
        if ( Helpers.LocalPlayer == GameState.Instance.PlayerOverlord )
        {
            if ( !Helpers.IsOverlordTurn() )
                return false;

            if ( GameState.Instance.ManaOverlord < cardDesc.GetManaToCast() )
                return false;
        }

        if ( Helpers.LocalPlayer == GameState.Instance.PlayerRunner )
        {
            if ( !Helpers.IsRunnerTurn() )
                return false;

            if ( GameState.Instance.ManaRunner < cardDesc.GetManaToCast() )
                return false;
        }

        return true;
    }
}
