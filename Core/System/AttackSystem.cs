using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;
using Slash.ECS.Inspector.Attributes;
using UnityEngine;

[GameSystem]
[InspectorType]
public class AttackSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        //this.EventManager.RegisterListener(RPGGameEvent.AttackHit, OnAttackHit);
        this.EventManager.RegisterListener(RPGGameEvent.Unity_OnAreaOfEffectStay, OnUnity_OnAreaOfEffectStay);
    }

    private void OnUnity_OnAreaOfEffectStay(GameEvent e)
    {
        var data = (EntityBehaviour.Unity_OnTriggerStayData)e.EventData;


        //var attack = this.EntityManager.GetComponent<AttackComponent>(data.AttackerID);
        //if (attack == null) { return; }

        //DamageTypes damageType;
        //DieRollData dieRoll;
        //float timedDamageModifier = 1.0f;

        //if (attack.AttackCounter == 0)
        //{
        //    dieRoll = attack.DamageInitial;
        //    damageType = attack.DamageTypeInitial;
        //}
        //else
        //{
        //    dieRoll = attack.DamageRecurring;
        //    damageType = attack.DamageTypeRecurring;
        //    timedDamageModifier = Time.deltaTime / GameBehavior.SecondsPerRound;
        //}
        //attack.AttackCounter++;
        
        //// Handle anything that effects the die roll (luck, etc.)

        //float damage = dieRoll.Total * timedDamageModifier;

        //// Handle anything that effects the damage (resistances, etc.)
        
        //this.EventManager.QueueEvent(RPGGameEvent.DamageTaken, new DamageSystem.DamageTaken() { EntityID = data.EntityID, AttackerID = data.AttackerID, Damage = damage });
    }

    public class AttackHit
    {
        public int EntityID;
        public int AttackerID;
    }
    public enum DamageTypes
    {
        Crush,
        Fire,
    }
}
