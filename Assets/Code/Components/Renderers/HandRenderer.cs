using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandRenderer: MonoBehaviour
{
    public GameObject CardPrefab;

    private IList<string> hand;

    private List<GameObject> cardGameObjects = new List<GameObject>();

    private bool mulliganFinished;

    void Start()
    {
    }

    void Update()
    {
        hand = null;

        if ( GameState.Instance == null )
            return;
        if ( gameObject != Helpers.LocalPlayer )
            return;

        if ( Helpers.LocalPlayer == GameState.Instance.PlayerOverlord )
            hand = GameState.Instance.HandOverlord;
        else if ( Helpers.LocalPlayer == GameState.Instance.PlayerRunner )
            hand = GameState.Instance.HandRunner;

        if ( hand == null )
            return;

        // Add/Update
        for ( int i = 0; i < hand.Count; ++i )
        {
            CardDesc card;
            if ( !CardDB.Instance.Cards.TryGetValue( hand[i], out card ) )
                continue;

            if ( cardGameObjects.Count <= i )
                cardGameObjects.Add( Instantiate( CardPrefab ) );

            cardGameObjects[i].GetComponent<CardInHand>().CardDesc = card;
            cardGameObjects[i].transform.localPosition = new Vector3( -6 + i * 1.1f, -4.3f, 0 );
        }

        // Remove
        var total = cardGameObjects.Count;
        for ( int i = total - 1; i >= hand.Count; --i )
        {
            Destroy( cardGameObjects[i] );
            cardGameObjects.RemoveAt( i );
        }
    }

    void OnGUI()
    {
        if ( GameState.Instance == null )
            return;
        if ( !GameState.Instance.IsMulliganState || mulliganFinished )
            return;
        if ( gameObject != Helpers.LocalPlayer )
            return;

        int x = 12;
        int y = 650;

        GUI.Label( new Rect( x, y, 200, 200 ),
            "Starting Hand: Click Card to Replace",
            new GUIStyle {fontSize = 20, normal = {textColor = Color.white}} );

        var mulliganDone = GUI.Button( new Rect( 300, 768 - 60 - 20 - 12, 100, 60 + 12 + 4 ), "Done" );
        if ( mulliganDone )
        {
            PlayerCommands.Instance.CmdMulliganFinished();
            mulliganFinished = true;
        }
    }
}
