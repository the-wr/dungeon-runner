using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SideSelectorPlayerHelper: NetworkBehaviour
{
    public void PickOverlord( GameObject sideSelector )
    {
        CmdPickOverlord( gameObject, sideSelector );
    }

    public void PickRunner( GameObject sideSelector )
    {
        CmdPickRunner( gameObject, sideSelector );
    }

    // -----

    [Command]
    private void CmdPickOverlord( GameObject playerObject, GameObject sideSelector )
    {
        sideSelector.GetComponent<SideSelector>().PickOverlord( playerObject );
    }

    [Command]
    private void CmdPickRunner( GameObject playerObject, GameObject sideSelector )
    {
        sideSelector.GetComponent<SideSelector>().PickRunner( playerObject );
    }
}
