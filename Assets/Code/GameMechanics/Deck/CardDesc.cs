using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public enum CardType
{
    Hero,
    Spell,
    Room,
    Heart,
    Enchantment
}

[Serializable]
[DebuggerDisplay( "{Id,nq} ({ManaCost,nq}) {Name,nq}" )]
public class CardDesc
{
    public string Id { get; set; }
    public string Name { get; set; }

    public int ManaCost { get; set; }

    public CardType Type { get; set; }
    public bool IsAmbush { get; set; }
    public bool IsDelayed { get; set; }

    public int Damage { get; set; }
    public int Health { get; set; }

    public List<EffectResource> Effects { get; set; }

    // -----

    public CardDesc()
    {
        Effects = new List<EffectResource>();
    }

    public int GetManaToCast()
    {
        if ( Type == CardType.Hero || Type == CardType.Spell )
            return ManaCost;

        return 0;
    }
}
