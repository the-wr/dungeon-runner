using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class RoomManager
{
    private readonly List<GameObject> roomSlots = new List<GameObject>();

    // Sorted by Order ASC
    private readonly List<List<GameObject>> roomSlotsByWing = new List<List<GameObject>>();

    private readonly Dictionary<GameObject, GameObject> slotsToRooms = new Dictionary<GameObject, GameObject>();

    public event Action<GameObject> RoomCreated;
    public event Action<GameObject> PreviewRoomDestroyed;
    public event Action<GameObject, RoomInfo, bool> RoomRevealed;

    public void Init()
    {
        RoomSlotsRenderer.InitRoomSlots( roomSlots, roomSlotsByWing );

        CreateHearts();
    }

    private void CreateHearts()
    {
        foreach ( var pair in CardDB.Instance.Cards )
        {
            if ( pair.Value.Type == CardType.Heart )
            {
                foreach ( var slots in roomSlotsByWing )
                    InstantiateRoomOrEnchantment( pair.Value, slots[1] );

                break;
            }
        }
    }

    public bool TryCastRoomInSlot( CardDesc cardDesc, GameObject targetSlot )
    {
        var ri = targetSlot.GetComponent<RoomSlotInfo>();
        if ( ri.IsHeart || ri.IsEnchantment )
            return false;

        // If slot before is empty - can't cast
        if ( ri.Order > 1 )
        {
            var slotBeforeThis = roomSlotsByWing[ri.Wing][ri.Order];
            if(!slotsToRooms.ContainsKey( slotBeforeThis ))
                return false;
        }

        InstantiateRoomOrEnchantment( cardDesc, targetSlot );
        return true;
    }

    public bool TryCastEnchantmentInSlot( CardDesc cardDesc, GameObject targetSlot )
    {
        var ri = targetSlot.GetComponent<RoomSlotInfo>();
        if ( !ri.IsEnchantment )
            return false;

        InstantiateRoomOrEnchantment( cardDesc, targetSlot );
        return true;
    }

    public GameObject GetFirstRoom( int wingNumber )
    {
        if ( wingNumber < 0 || wingNumber >= roomSlotsByWing.Count )
            return null;


        var wing = roomSlotsByWing[wingNumber];
        for ( int i = wing.Count - 1; i >= 1; --i )
        {
            if ( slotsToRooms.ContainsKey( wing[i] ) && !slotsToRooms[wing[i]].GetComponent<RoomInfo>().IsBroken )
                return slotsToRooms[wing[i]];
        }

        return null;
    }

    public GameObject GetNextRoom( GameObject room )
    {
        var ri = room.GetComponent<RoomInfo>();
        var si = ri.Slot.GetComponent<RoomSlotInfo>();

        if ( si.Order <= 0 )
            return null;

        var nextSlot = roomSlotsByWing[si.Wing][si.Order];
        return slotsToRooms.ContainsKey( nextSlot ) ? slotsToRooms[nextSlot] : null;
    }

    public void RestoreRooms()
    {
        var slotsToRoomsSafe = slotsToRooms.ToList();

        foreach ( var slotsToRoom in slotsToRoomsSafe )
        {
            var room = slotsToRoom.Value;
            var ri = room.GetComponent<RoomInfo>();

            CardDesc cardDesc;
            if ( !CardDB.Instance.Cards.TryGetValue( ri.CardId, out cardDesc ) )
                continue;

            if ( ri.IsBroken && cardDesc.Type == CardType.Enchantment )
                DestroyRoom( room );

            ri.DamageDealt = 0;
        }
    }

    public List<GameObject> GetHearts()
    {
        var result = new List<GameObject>();
        foreach ( var slots in roomSlotsByWing )
        {
            GameObject room;
            if ( slotsToRooms.TryGetValue( slots[1], out room ) )
                result.Add( room );
        }

        return result;
    }

    public GameObject GetEnchantmentInWing( int wing )
    {
        var slot = roomSlotsByWing[wing][0];
        return slotsToRooms.ContainsKey( slot ) ? slotsToRooms[slot] : null;
    }

    public GameObject GetHeartSlotInWing( int wing )
    {
        return roomSlotsByWing[wing][1];
    }

    public List<GameObject> GetAllRoomSlotsInWing( int wing )
    {
        return roomSlotsByWing[wing].Skip( 2 ).ToList();
    }

    public void DestroyRoom( GameObject room )
    {
        if ( PreviewRoomDestroyed != null )
            PreviewRoomDestroyed( room );

        GameObject.Destroy( room );
    }

    public void RevealRoom( GameObject room, RoomInfo roomInfo, bool isActive )
    {
        roomInfo.IsRevealed = true;

        if ( RoomRevealed != null )
            RoomRevealed( room, roomInfo, isActive );
    }

    public GameObject GetRoomInSlot( GameObject slot )
    {
        GameObject result;
        if ( slotsToRooms.TryGetValue( slot, out result ) )
            return result;

        return null;
    }

    // -----

    private GameObject InstantiateRoomOrEnchantment( CardDesc cardDesc, GameObject targetSlot )
    {
        GameObject previousRoom;
        if ( slotsToRooms.TryGetValue( targetSlot, out previousRoom ) && previousRoom != null )
        {
            DestroyRoom( previousRoom );
        }

        var newRoom = GameObject.Instantiate( GameRoot.Instance.RoomPrefab );
        var ri = newRoom.GetComponent<RoomInfo>();
        ri.Slot = targetSlot;
        ri.CardId = cardDesc.Id;

        ri.Damage = cardDesc.Damage;
        ri.DamageDealt = 0;
        ri.TotalHealth = cardDesc.Health;

        if ( cardDesc.Type == CardType.Heart )
        {
            ri.IsRevealed = true;
            ri.IsActive = true;
        }

        slotsToRooms[targetSlot] = newRoom;

        if ( RoomCreated != null )
            RoomCreated( newRoom );

        NetworkServer.Spawn( newRoom );

        return newRoom;
    }
}
