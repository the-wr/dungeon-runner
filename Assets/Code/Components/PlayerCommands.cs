using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerCommands: NetworkBehaviour
{
    public static PlayerCommands Instance
    {
        get { return Helpers.LocalPlayer.GetComponent<PlayerCommands>(); }
    }

    [Command]
    public void CmdEndTurn()
    {
        GameRoot.Instance.PhaseManager.EndTurn();
    }

    [Command]
    public void CmdPlayCard( string id, GameObject targetSlot )
    {
        GameRoot.Instance.Mechanics.PlayCard( gameObject, id, targetSlot );
    }

    [Command]
    public void CmdActivateRoom( GameObject room )
    {
        GameRoot.Instance.Mechanics.ActivateRoom( gameObject, room );
    }

    [Command]
    public void CmdStartRun( GameObject roomSlot )
    {
        GameRoot.Instance.Mechanics.StartRun( gameObject, roomSlot );
    }

    [Command]
    public void CmdEnterRoom()
    {
        GameRoot.Instance.Mechanics.EnterRoom();
    }

    [Command]
    public void CmdRetreat()
    {
        GameRoot.Instance.Mechanics.Retreat();
    }

    [Command]
    public void CmdAttackWithHero( GameObject hero )
    {
        GameRoot.Instance.Mechanics.AttackWithHero( hero );
    }

    [Command]
    public void CmdMulliganFinished()
    {
        GameRoot.Instance.PhaseManager.MulliganFinished( gameObject );
    }

    [Command]
    public void CmdMulliganReplaceCard( string cardId )
    {
        GameRoot.Instance.Mechanics.MulliganReplaceCard( gameObject, cardId );
    }
}
