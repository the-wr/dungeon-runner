using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SideSelector: NetworkBehaviour
{
    [SyncVar]
    private GameObject playerOverlord;
    [SyncVar]
    private GameObject playerRunner;

    public event Action ReadyToStart;

    public GameObject PlayerOverlord { get { return playerOverlord; } }

    public GameObject PlayerRunner { get { return playerRunner; } }

    void OnGUI()
    {
        {
            var text = "SideSelector";
            if ( isServer )
                text += " - Server";
            else if ( isClient )
                text += " - Client";

            GUI.Label( new Rect( 10, 50, 200, 100 ), text );
        }

        // ----- Side picker

        if ( !isServer )
        {
            int w = 200;
            int h = 100;

            GUI.BeginGroup( new Rect( Screen.width / 2 - w / 2, Screen.height / 2 - h / 2, w, h ) );
            GUI.Box( new Rect( 0, 0, w, h ), "Pick Your Side" );

            var pickRunner = GUI.Button( new Rect( 10, 30, 85, 40 ), "Runner" );
            var pickOverlord = GUI.Button( new Rect( 105, 30, 85, 40 ), "Overlord" );

            GUI.Label( new Rect( 10, 70, 85, 30 ), GetName( playerRunner ), Helpers.TextCenter );
            GUI.Label( new Rect( 105, 70, 85, 30 ), GetName( playerOverlord ), Helpers.TextCenter );

            GUI.EndGroup();

            if ( pickOverlord )
                MatchMaker.Instance.LocalPlayer.GetComponent<SideSelectorPlayerHelper>().PickOverlord( gameObject );

            if ( pickRunner )
                MatchMaker.Instance.LocalPlayer.GetComponent<SideSelectorPlayerHelper>().PickRunner( gameObject );
        }
    }

    public void PickOverlord( GameObject player )
    {
        if ( playerRunner == player )
            playerRunner = null;

        playerOverlord = player;

        CheckStart();
    }

    public void PickRunner( GameObject player )
    {
        if ( playerOverlord == player )
            playerOverlord = null;

        playerRunner = player;

        CheckStart();
    }

    private void CheckStart()
    {
        if ( playerOverlord != null && playerRunner != null && ReadyToStart != null )
            ReadyToStart();
    }

    private string GetName( GameObject gameObject )
    {
        if ( gameObject == null )
            return string.Empty;

        var pi = gameObject.GetComponent<PlayerInfo>();
        if ( pi == null )
            return string.Empty;

        return pi.Name;
    }
}
