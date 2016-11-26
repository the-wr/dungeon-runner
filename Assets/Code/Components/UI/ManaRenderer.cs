using System;
using UnityEngine;
using System.Collections;

public class ManaRenderer: MonoBehaviour
{

    void OnGUI()
    {
        if ( GameState.Instance == null )
            return;
        if ( gameObject != Helpers.LocalPlayer )
            return;
        if ( GameState.Instance.IsMulliganState )
            return;

        int x = 12;
        int y = 650;

        if ( gameObject == GameState.Instance.PlayerRunner )
        {
            GUI.Label( new Rect( x, y, 200, 200 ),
                string.Format( "Mana: {0}/{1}" /*+ "     Overlord Cards: {2}"*/, GameState.Instance.ManaRunner, GameState.Instance.MaxManaRunner/*,
                GameState.Instance.HandOverlord.Count*/ ),
                new GUIStyle { fontSize = 20, normal = { textColor = Color.white } } );
        }
        if ( gameObject == GameState.Instance.PlayerOverlord )
        {
            GUI.Label( new Rect( x, y, 600, 200 ),
                string.Format( "Mana: {0}/{1}" /*+ "     Runner Mana: {2}/{3}     Runner Cards: {4}"*/,
                    GameState.Instance.ManaOverlord, GameState.Instance.MaxManaOverlord/*,
                    GameState.Instance.ManaRunner, GameState.Instance.MaxManaRunner,
                    GameState.Instance.HandRunner.Count*/ ),
                new GUIStyle { fontSize = 20, normal = { textColor = Color.white } } );
        }
    }
}
