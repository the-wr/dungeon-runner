using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
[XmlInclude( typeof( EffectBonusStats ) )]
public class EffectResource
{
    public EffectTargetSelector TargetSelector { get; set; }

    public virtual EffectInstance CreateInstance() { return new EffectInstance(); }
}

public class EffectInstance: IDisposable
{
    public virtual void Attach( GameObject slot )
    {
    }

    public virtual void Detach()
    {
    }

    public void Dispose()
    {
    }
}

[Serializable]
[XmlInclude( typeof( TargetSelectorSelf ) )]
[XmlInclude( typeof( TargetSelectorHeartInSameWing ) )]
[XmlInclude( typeof( TargetSelectorAllRoomsInSameWing ) )]
public class EffectTargetSelector
{
    // Returns slot objects
    public virtual List<GameObject> GetTargets( GameObject slot ) { return new List<GameObject>(); }
}

public class TargetSelectorSelf: EffectTargetSelector
{
    public override List<GameObject> GetTargets( GameObject slot )
    {
        return new List<GameObject> { slot };
    }
}

public class TargetSelectorHeartInSameWing: EffectTargetSelector
{
    public override List<GameObject> GetTargets( GameObject slot )
    {
        var heartSlot = GameRoot.Instance.Mechanics.RoomManager.GetHeartSlotInWing( slot.GetComponent<RoomSlotInfo>().Wing );
        if ( heartSlot == null )
            return base.GetTargets( slot );

        return new List<GameObject> { heartSlot };
    }
}

public class TargetSelectorAllRoomsInSameWing: EffectTargetSelector
{
    public override List<GameObject> GetTargets( GameObject slot )
    {
        return GameRoot.Instance.Mechanics.RoomManager.GetAllRoomSlotsInWing( slot.GetComponent<RoomSlotInfo>().Wing );
    }
}