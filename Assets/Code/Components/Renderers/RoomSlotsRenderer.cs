using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RoomSlotsRenderer: MonoBehaviour
{
    private List<GameObject> roomSlots = new List<GameObject>();

    // Sorted by Order ASC
    private readonly List<List<GameObject>> roomSlotsByWing = new List<List<GameObject>>();

    private readonly Dictionary<GameObject, GameObject> slotsToRooms = new Dictionary<GameObject, GameObject>();
    private readonly Dictionary<GameObject, Vector3> slotsToInitialPos = new Dictionary<GameObject, Vector3>();

    public void UnhighlightAll()
    {
        foreach ( var room in roomSlots )
        {
            var h = room.GetComponent<Highlightable>();
            if ( h != null )
                h.IsHighlighted = false;
        }

        SetAllRoomHitTest( true );
    }

    public void HighlightForNewRoom()
    {
        SetAllRoomHitTest( false );

        foreach ( var slots in roomSlotsByWing )
        {
            bool prevSlotHasRoom = false;
            for ( int i = 2; i < slots.Count; ++i )
            {
                var hasRoom = slotsToRooms.ContainsKey( slots[i] );
                if ( i == 2 || hasRoom || prevSlotHasRoom )
                {
                    var highlightable = slots[i].GetComponent<Highlightable>();
                    highlightable.IsHighlighted = true;
                    highlightable.Color = hasRoom ? Helpers.ColorSelectorReplaceRoom : Helpers.ColorSelectorNewRoom;
                }

                prevSlotHasRoom = hasRoom;
            }
        }
    }

    public void HightlightForAttack()
    {
        SetAllRoomHitTest( false );

        foreach ( var slots in roomSlotsByWing )
        {
            for ( int i = slots.Count - 1; i >= 0; --i )
            {
                if ( slotsToRooms.ContainsKey( slots[i] ) )
                {
                    var highlightable = slots[i].GetComponent<Highlightable>();
                    highlightable.IsHighlighted = true;
                    highlightable.Color = Helpers.ColorSelectorAttackRoom;
                    break;
                }
            }
        }
    }

    public void HighlightForNewEnchantment()
    {
        foreach ( var slots in roomSlotsByWing )
        {
            var hasEnch = slotsToRooms.ContainsKey( slots[0] );
            var highlightable = slots[0].GetComponent<Highlightable>();
            highlightable.IsHighlighted = true;

            highlightable.Color = hasEnch ? Helpers.ColorSelectorReplaceRoom : Helpers.ColorSelectorNewRoom;
        }
    }

    public void OnRoomSpawned( GameObject slot, GameObject room )
    {
        slotsToRooms[slot] = room;
    }

    public void OnRoomDestroyed( GameObject slot, GameObject room )
    {
        GameObject prevRoom;
        if ( slotsToRooms.TryGetValue( slot, out prevRoom ) && prevRoom == room )
            slotsToRooms.Remove( slot );
    }

    public void SetAllRoomHitTest( bool enabled )
    {
        foreach ( var slotsToRoom in slotsToRooms )
        {
            slotsToRoom.Value.GetComponent<BoxCollider2D>().enabled = enabled;
        }
    }

    void Start()
    {
        if ( Helpers.LocalPlayer != gameObject )
            return;

        InitRoomSlots( roomSlots, roomSlotsByWing );

        foreach ( var roomSlot in roomSlots )
            slotsToInitialPos[roomSlot] = roomSlot.transform.position;

        UnhighlightAll();
    }

    void Update()
    {
        if ( GameState.Instance == null )
            return;
        if ( Helpers.LocalPlayer != gameObject )
            return;

        int runWing = -1;
        int runOrder = -1;

        bool isInRun = GameState.Instance.RunState != null;
        if ( isInRun )
        {
            var slotInfo = GameState.Instance.RunState.GetComponent<RunState>()
                .Room.GetComponent<RoomInfo>()
                .Slot.GetComponent<RoomSlotInfo>();

            if ( slotInfo != null )
            {
                runWing = slotInfo.Wing;
                runOrder = slotInfo.Order;
            }
        }

        foreach ( var pair in slotsToInitialPos )
        {
            var slot = pair.Key;
            var slotInfo = slot.GetComponent<RoomSlotInfo>();

            if ( slotInfo.Wing == runWing && slotInfo.Order > runOrder )
                slot.transform.position = new Vector3( pair.Value.x - 2.5f, pair.Value.y );
            else
                slot.transform.position = pair.Value;

            GameObject room;
            if ( slotsToRooms.TryGetValue( slot, out room ) )
                room.transform.position = slot.transform.position;
        }
    }

    public static void InitRoomSlots( List<GameObject> roomSlots, List<List<GameObject>> roomSlotsByWing )
    {
        roomSlots.AddRange( GameObject.FindGameObjectsWithTag( "RoomSlot" ).ToList() );
        roomSlots.Sort( ( r1, r2 ) => r1.GetComponent<RoomSlotInfo>().Wing.CompareTo( r2.GetComponent<RoomSlotInfo>().Wing ) );

        foreach ( var room in roomSlots )
        {
            var ri = room.GetComponent<RoomSlotInfo>();
            if ( roomSlotsByWing.Count <= ri.Wing )
                roomSlotsByWing.Add( new List<GameObject>() );

            roomSlotsByWing[ri.Wing].Add( room );
        }

        foreach ( var roomsInWing in roomSlotsByWing )
        {
            roomsInWing.Sort( ( r1, r2 ) => r1.GetComponent<RoomSlotInfo>().Order.CompareTo( r2.GetComponent<RoomSlotInfo>().Order ) );
        }
    }

}
