using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class RoomInfo: NetworkBehaviour
{
    [SyncVar]
    public GameObject Slot;
    [SyncVar]
    public string CardId;
    [SyncVar]
    public bool IsActive;
    [SyncVar]
    public bool IsRevealed;

    [SyncVar]
    public int Damage;
    [SyncVar]
    public int DamageDealt;
    [SyncVar]
    public int TotalHealth;

    // -----

    public CardDesc CardDesc;

    // -----

    [SyncVar]
    private bool forceBroken;

    // -----

    public void Start()
    {
        var rr = GetComponent<RoomRenderer>();
        if ( rr != null && ( Helpers.IsLocalPlayerOverlord() || Helpers.IsLocalPlayerRunner() ) )
            rr.enabled = true;

        CardDB.Instance.Cards.TryGetValue( CardId, out CardDesc );
    }

    public bool IsBroken
    {
        get { return ( CardDesc.Type == CardType.Room || CardDesc.Type == CardType.Heart ) && ( DamageDealt >= TotalHealth ) || forceBroken; }
        set { forceBroken = value; }
    }
}
