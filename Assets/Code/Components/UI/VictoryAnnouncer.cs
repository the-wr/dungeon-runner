using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class VictoryAnnouncer: NetworkBehaviour
{
    [SyncVar]
    public string Victor;

    void OnGUI()
    {
        GUI.Box( new Rect( 0, 0, Screen.width, Screen.height ), "" );
        GUI.Box( new Rect( 0, 0, Screen.width, Screen.height ), "" );

        GUI.Label( new Rect( 0, 0, Screen.width, Screen.height ), Victor + " wins!",
            new GUIStyle { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white }, fontSize = 100 } );

        if ( GUI.Button( new Rect( 300, 600, Screen.width - 300 * 2, 50 ), "Exit" ) )
            Application.Quit();
    }
}
