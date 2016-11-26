using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.Networking;

public class GameRoot: NetworkBehaviour
{
    private static GameRoot instance;

    public GameObject SideSelectorPrefab;
    public GameObject GameStatePrefab;

    public GameObject TurnOverlordPrefab;
    public GameObject TurnRunnerPrefab;
    public GameObject RunStatePrefab;
    public GameObject VictoryAnnouncerPrefab;

    public GameObject RoomPrefab;
    public GameObject HeroPrefab;

    public GameObject DialogActivateRoomPrefab;

    private GameObject sideSelectorObj;
    private GameObject gameStateObj;

    private Mechanics mechanics;
    private PhaseManager phaseManager;

    public GameRoot()
    {
        instance = this;
    }

    public static GameRoot Instance { get { return instance; } }

    public PhaseManager PhaseManager { get { return phaseManager; } }
    public Mechanics Mechanics { get { return mechanics; } }

    void Start()
    {
        if ( !isServer )
            return;

        sideSelectorObj = Instantiate( SideSelectorPrefab );
        NetworkServer.Spawn( sideSelectorObj );

        var sideSelector = sideSelectorObj.GetComponent<SideSelector>();
        sideSelector.ReadyToStart += OnReadyToStart;
    }

    // Server
    private void OnReadyToStart()
    {
        var sideSelector = sideSelectorObj.GetComponent<SideSelector>();

        gameStateObj = Instantiate( GameStatePrefab );

        var gameState = gameStateObj.GetComponent<GameState>();
        gameState.Setup( sideSelector.PlayerOverlord, sideSelector.PlayerRunner );

        NetworkServer.Spawn( gameStateObj );

        phaseManager = gameObject.AddComponent<PhaseManager>();

        mechanics = new Mechanics();
        mechanics.StartGame( gameState, phaseManager );

        phaseManager.StartMulliganPhase();

        Destroy( sideSelectorObj );
    }
}
