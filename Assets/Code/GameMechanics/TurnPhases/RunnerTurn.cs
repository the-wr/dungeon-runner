using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class RunnerTurn: NetworkBehaviour
{
    void OnGUI()
    {
        Helpers.StateLabel( this );

        if ( Helpers.LocalPlayer == GameState.Instance.PlayerRunner )
        {
            var endTurn = Helpers.EndTurnButton();
            if ( endTurn )
                PlayerCommands.Instance.CmdEndTurn();
        }
    }
}
