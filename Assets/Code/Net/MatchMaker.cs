using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MatchMaker: NetworkManager
{
    private static MatchMaker instance;
    public static MatchMaker Instance { get { return instance; } }

    private ListMatchResponse listMatchResponce;
    private bool doSearch = true;

    private Scene startScene;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad( gameObject );
        StartMatchMaker();

        CardDB.Instance.Init();

        startScene = SceneManager.GetActiveScene();
        matchMaker.SetProgramAppID( (AppID)1156002 );

        StartCoroutine( DoGetMatchList() );
    }

    private void OnGUI()
    {
        if( SceneManager.GetActiveScene() != startScene )
            return;

        if ( !doSearch )
        {
            GUI.Label( new Rect( Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200 ),
                "Connecting...\r\n\r\nThis may take up to 10 seconds",
                new GUIStyle() { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } } );

            return;
        }

        if ( listMatchResponce == null || !listMatchResponce.success )
        {
            int w = 300;
            int h = 100;

            GUI.BeginGroup( new Rect( Screen.width / 2 - w / 2, Screen.height / 2 - h / 2, w, h ) );
            GUI.Box( new Rect( 0, 0, w, h ), "Connecting to MatchMaking server..." );
            GUI.Label( new Rect( 10, 30, w - 20, h - 30 ),
                listMatchResponce == null ? "Connecting" : listMatchResponce.extendedInfo, Helpers.TextCenter );
            GUI.EndGroup();
        }
        else
        {
            int w = 300;
            int buttonH = 30;
            int h = listMatchResponce.matches.Count * buttonH + 50;

            GUI.BeginGroup( new Rect( Screen.width / 2 - w / 2, Screen.height / 2 - h / 2, w, h + buttonH + 10 ) );
            GUI.Box( new Rect( 0, 0, w, h ), "Join Game" );

            for ( int i = 0; i < listMatchResponce.matches.Count; ++i )
            {
                var m = listMatchResponce.matches[i];
                var joinThis = GUI.Button( new Rect( 10, 30 + buttonH * i, w - 20, buttonH ),
                    string.Format( "{0} ({1}/{2})", m.name, m.currentSize - 1, m.maxSize - 1 ) );

                if ( joinThis )
                {
                    doSearch = false;
                    matchMaker.JoinMatch( m.networkId, "", OnMatchJoined );
                }
            }

            var startServer = GUI.Button( new Rect( 0, h + 10, w, buttonH ), "Start Server" );
            GUI.EndGroup();

            if ( startServer )
            {
                doSearch = false;
                CreateMatch();
            }
        }
    }

    public GameObject LocalPlayer { get { return client.connection.playerControllers[0].gameObject; } }

    // -----

    private IEnumerator DoGetMatchList()
    {
        while ( doSearch )
        {
            matchMaker.ListMatches( 0, 20, "", OnMatchList );
            yield return new WaitForSeconds( 2 );
        }
    }

    private void CreateMatch()
    {
        doSearch = false;
        var mn = string.Format( "{0} - {1:00}:{2:00}",
            Environment.UserName, DateTime.Now.Hour, DateTime.Now.Minute );

        matchMaker.CreateMatch( mn, 3, true, "", OnMatchCreate );
    }

    // ----- Both

    public override void OnMatchList( ListMatchResponse response )
    {
        base.OnMatchList( response );

        listMatchResponce = response;
    }
}