using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerInfo: NetworkBehaviour
{
    [SyncVar]
    public string Name;

    void Start()
    {
        if ( !isLocalPlayer )
            return;

        CmdSetInfo( Environment.UserName );
    }

    [Command]
    void CmdSetInfo( string playerName )
    {
        Name = playerName;
    }
}
