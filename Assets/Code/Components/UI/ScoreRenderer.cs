using UnityEngine;
using System.Collections;

public class ScoreRenderer: MonoBehaviour
{
    void OnGUI()
    {
        if ( GameState.Instance == null )
            return;
        if ( gameObject != Helpers.LocalPlayer )
            return;

        GUI.Label( new Rect( 0, 620, Screen.width - 12, 100 ),
            string.Format( "Overlord Life: {0}", GameState.Instance.OverlordLife ),
            new GUIStyle { alignment = TextAnchor.UpperRight, normal = { textColor = Color.white }, fontSize = 20 } );
        GUI.Label( new Rect( 0, 650, Screen.width - 12, 100 ),
            string.Format( "Masterplan Steps Left: {0}", GameState.Instance.OverlordVictorySteps ),
            new GUIStyle { alignment = TextAnchor.UpperRight, normal = { textColor = Color.white }, fontSize = 20 } );
    }
}