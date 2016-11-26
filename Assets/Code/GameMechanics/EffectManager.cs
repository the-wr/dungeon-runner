using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlotMechanics
{
    public int BonusDamage { get; set; }
    public int BonusHealth { get; set; }
    /*
    private readonly GameObject slot;

    public GameObject Slot { get { return slot; } }

    public SlotMechanics( GameObject slot )
    {
        this.slot = slot;
    }
    */
}

public class EffectData
{
    public EffectInstance EffectInstance { get; set; }
    public GameObject Slot { get; set; }
}

public class EffectManager
{
    private readonly Dictionary<GameObject, SlotMechanics> slotMechanics = new Dictionary<GameObject, SlotMechanics>();
    private readonly Dictionary<SlotMechanics, GameObject> slotMechanicsToSlot = new Dictionary<SlotMechanics, GameObject>();

    private readonly Dictionary<GameObject, List<EffectData>> roomEffects = new Dictionary<GameObject, List<EffectData>>();

    private RoomManager roomManager;

    public void Init( RoomManager roomManager )
    {
        this.roomManager = roomManager;

        roomManager.RoomRevealed += OnRoomRevealed;
        roomManager.RoomCreated += OnRoomCreated;
        roomManager.PreviewRoomDestroyed += OnRoomDestroyed;

        foreach ( var slot in GameObject.FindGameObjectsWithTag( "RoomSlot" ) )
        {
            var sm = new SlotMechanics();

            slotMechanics[slot] = sm;
            slotMechanicsToSlot[sm] = slot;
            //sm.RecalculateRequest += OnRecalculateSlot;
        }
    }

    public SlotMechanics GetSlotMechanics( GameObject slot )
    {
        SlotMechanics result;
        if ( slotMechanics.TryGetValue( slot, out result ) )
            return result;

        return null;
    }

    // -----

    private void OnRoomRevealed( GameObject room, RoomInfo roomInfo, bool isActive )
    {
        if ( !isActive )
            return;

        CardDesc cardDesc;
        if ( !CardDB.Instance.Cards.TryGetValue( roomInfo.CardId, out cardDesc ) )
            return;

        ApplyEffects( room, roomInfo.Slot, cardDesc );
    }

    private void OnRoomCreated( GameObject room )
    {
        RecalculateSlot( room.GetComponent<RoomInfo>().Slot );
    }

    private void OnRoomDestroyed( GameObject room )
    {
        RemoveEffects( room );
    }

    private void ApplyEffects( GameObject room, GameObject slot, CardDesc cardDesc )
    {
        var effects = new List<EffectData>();
        roomEffects[room] = effects;

        foreach ( var effectResource in cardDesc.Effects )
        {
            var ts = effectResource.TargetSelector;
            if ( ts == null )
                continue;

            var targets = ts.GetTargets( slot );

            foreach ( var targetSlot in targets )
            {
                var instance = effectResource.CreateInstance();
                instance.Attach( targetSlot );

                effects.Add( new EffectData { EffectInstance = instance, Slot = targetSlot } );

                RecalculateSlot( targetSlot );
            }
        }
    }

    private void RemoveEffects( GameObject room )
    {
        List<EffectData> effects;
        if ( !roomEffects.TryGetValue( room, out effects ) )
            return;

        foreach ( var effectData in effects )
        {
            effectData.EffectInstance.Detach();
            RecalculateSlot( effectData.Slot );
        }
    }

    private void RecalculateSlot( GameObject slot )
    {
        var room = roomManager.GetRoomInSlot( slot );
        if ( room == null )
            return;

        var sm = slotMechanics[slot];
        var ri = room.GetComponent<RoomInfo>();

        CardDesc cardDesc;
        if ( !CardDB.Instance.Cards.TryGetValue( ri.CardId, out cardDesc ) )
            return;

        ri.Damage = cardDesc.Damage + sm.BonusDamage;
        ri.TotalHealth = cardDesc.Health + sm.BonusHealth;
    }
}
