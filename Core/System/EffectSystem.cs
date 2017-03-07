using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;
using Slash.ECS.Inspector.Attributes;
using UnityEngine;

[GameSystem]
[InspectorType]
public class EffectSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        //this.EventManager.RegisterListener(RPGGameEvent.Unity_OnAreaOfEffectStay, OnUnity_OnAreaOfEffectStay);
    }

    //private void OnUnity_OnAreaOfEffectStay(GameEvent e) 
    //{
    //    var data = (EntityBehaviour.Unity_OnTriggerStayData)e.EventData;

    //    // AttackComponent
    //    var attack = this.EntityManager.GetComponent<AttackComponent>(data.ItemID);
    //    if (attack != null)
    //    {
    //        this.EventManager.QueueEvent(RPGGameEvent.AttackHit, new AttackSystem.AttackHit() { EntityID = data.ActorID, AttackerID = data.ItemID });
    //    }
    //}

    public static void CreateEffectEntity(EntityManager em, EffectData data) 
    {
        if (data == null) { return; }

        int id = em.CreateEntity();

        FormComponent form = null;
        DurationComponent duration = null;

        // FormComponent
        if (data.PrefabName != "")
        {
            form = new FormComponent();
            form.InitComponent(id, data.Location, data.PrefabName, "");
            em.AddComponent(id, form);

            // AreaOfEffectComponent
            if (data.TargetType == TargetTypeEnum.AreaOfEffect) 
            {
                form.SetSphereColliderRadius(data.AreaOfEffectRadius);
            }
        }

        // DurationComponent
        if (data.Duration >= 0)
        {
            duration = new DurationComponent(); 
            duration.InitComponent(data.Duration, false);
            em.AddComponent(id, duration);
        }

        // AttackComponent
        if (data.Description_Attack != "")
        {
            //var attack = new AttackComponent() { Damage = data.Damage, DamageType = data.DamageType, Description = data.Description_Attack };
            //em.AddComponent(id, attack);
        }

        if (duration != null) { duration.TimerActive = true; }
        if (form != null) { form.SetActive(true); }
    }

    public class EffectData 
    {
        // For AreaOfEffectComponent
        public float          AreaOfEffectRadius;
        public TargetTypeEnum TargetType;

        // For DurationComponent
        public float  Duration =-1.0f; // -1 means no duration

        // For FormComponent
        public Vector3 Location;
        public string  PrefabName;

        // For AttackComponent
        public DieRollData              Damage;
        public AttackSystem.DamageTypes DamageType;
        public string                   Description_Attack;
    }
    public enum TargetTypeEnum 
    {
        Actor,
        AreaOfEffect
    }
}
