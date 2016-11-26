using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HeroInfo: NetworkBehaviour
{
    [SyncVar]
    public string CardId;

    [SyncVar]
    public int Damage;
    [SyncVar]
    public int Health;

    [SyncVar]
    public bool SummoningSickness;

    [SyncVar]
    public int Frags;

    public void Start()
    {
        var hr = GetComponent<HeroRenderer>();
        if ( hr != null && ( Helpers.IsLocalPlayerOverlord() || Helpers.IsLocalPlayerRunner() ) )
            hr.enabled = true;
    }
}
