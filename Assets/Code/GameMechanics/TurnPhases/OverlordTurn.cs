using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class OverlordTurn: NetworkBehaviour
{
    void OnGUI()
    {
        Helpers.StateLabel( this );

        if ( Helpers.LocalPlayer == GameState.Instance.PlayerOverlord )
        {
            var endTurn = Helpers.EndTurnButton();
            if ( endTurn )
                PlayerCommands.Instance.CmdEndTurn();
        }
    }
}
