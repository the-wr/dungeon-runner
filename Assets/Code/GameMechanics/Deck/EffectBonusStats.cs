using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class EffectBonusStats: EffectResource
{
    public int Damage { get; set; }
    public int Health { get; set; }

    public override EffectInstance CreateInstance()
    {
        return new Instance( this );
    }

    public class Instance: EffectInstance
    {
        private readonly EffectBonusStats desc;
        private SlotMechanics slotMechanics;

        public Instance( EffectBonusStats desc )
        {
            this.desc = desc;
        }

        public override void Attach( GameObject slot )
        {
            slotMechanics = GameRoot.Instance.Mechanics.EffectManager.GetSlotMechanics( slot );
            if ( slotMechanics == null )
                return;

            slotMechanics.BonusDamage += desc.Damage;
            slotMechanics.BonusHealth += desc.Health;
        }

        public override void Detach()
        {
            if ( slotMechanics == null )
                return;

            slotMechanics.BonusDamage -= desc.Damage;
            slotMechanics.BonusHealth -= desc.Health;

            slotMechanics = null;
        }
    }
}