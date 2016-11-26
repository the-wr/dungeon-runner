using System;
using UnityEngine;
using System.Collections;

public class ActivateRoomDialog: MonoBehaviour
{
    public GameObject Room { get; set; }
    public CardDesc CardDesc { get; set; }

    public event Action Done;

    void OnGUI()
    {
        if ( Room == null )
            return;

        var roomRect = Helpers.GetScreenSpaceBounds( Room );
        var rect = new Rect( roomRect.x - 10, roomRect.y - 10, roomRect.width + 20, roomRect.height + 20 );

        GUI.Box( rect, "" );
        GUI.Box( rect, "Activate room?" );

        GUI.Label( new Rect(rect.x, rect.y + 30, rect.width, 30), "Mana cost: " + CardDesc.ManaCost, Helpers.TextCenter );
        var btnActivate = GUI.Button( new Rect( rect.x + 5, rect.y + 60, rect.width - 10, 30 ), "Activate" );
        var btnCancel = GUI.Button( new Rect( rect.x + 5, rect.y + 100, rect.width - 10, 30 ), "Cancel" );

        if ( btnActivate )
        {
            Destroy( gameObject );

            PlayerCommands.Instance.CmdActivateRoom( Room );

            if ( Done != null )
                Done();
        }

        if ( btnCancel )
        {
            Destroy( gameObject );

            if ( Done != null )
                Done();
        }
    }
}
