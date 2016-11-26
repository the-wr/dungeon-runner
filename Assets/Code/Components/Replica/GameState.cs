using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class GameState: NetworkBehaviour
{
    private static GameState instance;

    [SyncVar]
    private GameObject playerOverlord;
    [SyncVar]
    private GameObject playerRunner;

    [SyncVar]
    public bool IsMulliganState;
    [SyncVar]
    public GameObject RunState;

    [SyncVar]
    public int OverlordLife = Constants.OverlordLife;
    [SyncVar]
    public int OverlordVictorySteps = Constants.OverlordVictorySteps;

    //[SyncVar(hook = "OnRoundNumberUpdated")]
    [SyncVar]
    public int RoundNumber;
    [SyncVar]
    public int ManaOverlord;
    [SyncVar]
    public int MaxManaOverlord;
    [SyncVar]
    public int ManaRunner;
    [SyncVar]
    public int MaxManaRunner;

    public SyncListString HandOverlord = new SyncListString();
    public SyncListString HandRunner = new SyncListString();

    // ----- Server only

    public List<string> DeckOverlord = new List<string>();
    public List<string> DeckRunner = new List<string>();

    // -----

    public static GameState Instance { get { return instance; } }

    public GameObject PlayerOverlord { get { return playerOverlord; } }
    public GameObject PlayerRunner { get { return playerRunner; } }

    // -----

    public void Start()
    {
        instance = this;
    }

    void OnGUI()
    {
        {
            var text = "GameState";
            if ( isServer )
                text += " - Server";
            if ( Helpers.LocalPlayer == playerOverlord )
                text += " - Overlord";
            if ( Helpers.LocalPlayer == playerRunner )
                text += " - Runner";

            GUI.Label( new Rect( 10, 30, 200, 100 ), text );
        }
    }

    // -----

    // Server
    public void Setup( GameObject playerOverlord, GameObject playerRunner )
    {
        instance = this;

        this.playerOverlord = playerOverlord;
        this.playerRunner = playerRunner;
    }

    // -----
    /*
    public void OnRoundNumberUpdated( int value )
    {
        RoundNumber = value;
    }
    */
}
