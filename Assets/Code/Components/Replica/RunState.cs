using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public enum RunStage
{
    Approach,
    Combat
}

public class RunState: NetworkBehaviour
{
    [SyncVar]
    public GameObject Room;
    [SyncVar]
    public RunStage Stage;
}
