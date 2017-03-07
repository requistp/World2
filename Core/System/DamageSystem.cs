using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;
using Slash.ECS.Inspector.Attributes;
using System.Collections.Generic;
using UnityEngine;

[GameSystem]
[InspectorType]
public class DamageSystem : GameSystem
{
    public const float DistanceTargetable = 8.5f;
    private static readonly List<RPGGameEvent> AllMenuOptions = new List<RPGGameEvent>() { RPGGameEvent.Menu_AttackMelee };

    public override void Init(IAttributeTable configuration) 
    {
        //this.EventManager.RegisterListener(RPGGameEvent.Input_Mouse_Hit, OnMouseHit);
        this.EventManager.RegisterListener(RPGGameEvent.DamageTaken, OnDamageTaken);
    }

    private void OnDamageTaken(GameEvent e) 
    {
        var data = (DamageTaken)e.EventData;

        var health = this.EntityManager.GetComponent<HealthComponent>(data.EntityID);
        if (health == null) { return; }
        if (health.DamageDisabled) { return; }

        health.Damage += data.Damage;

        Debug.Log(string.Format("Dead: Damage Taken {0} by {1} damage={2} HP={3}", data.EntityID, data.AttackerID, data.Damage, health.HP_Current));

        if (health.HP_Current <= 0)
        {
            //Debug.Log(string.Format("Dead: Damage Taken {0} by {1} damage={2} HP={3}", data.EntityID, data.AttackerID, data.Damage, health.HP_Current));

            switch (health.DeathType)
            {
                case HealthComponent.DeathTypes.Broken:
                    OnBroken(data.EntityID);
                    break;
            }
        }
    }
    private void OnMouseHit(GameEvent e) 
    {
        var data = (InputMouseBehaviour.MouseData)e.EventData;

        SetMenuOptions(data.EntityID, data.HitEntityID);
    }

    private void OnBroken(int entityID) 
    {
        // Check for anything "inside" the item that should be saved?

        // Replace FormComponent?
        FormComponent newForm = null;
        var form = this.EntityManager.GetComponent<FormComponent>(entityID);
        if (form != null && form.PrefabDeath != "")
        {
            newForm = new FormComponent();
            newForm.InitComponent(entityID, form.Position_Center, form.PrefabDeath, "");
        }

        // Other Components that suruve should be copied here

        // Remove components unless of a type to keep
        var removeList = this.EntityManager.GetComponents(entityID);
        foreach (var comp in removeList)
        {
            this.EntityManager.RemoveComponent(entityID, comp.GetType());
        }

        // Add new Components
        if (newForm != null) { this.EntityManager.AddComponent(entityID, newForm); }

        this.EventManager.QueueEvent(RPGGameEvent.Death_Broken, new Broken() { EntityID = entityID });
    }
    private void SetMenuOptions(int actorID, int itemID)
    {
        if (!MenuSystem.RemoveOptions(actorID, AllMenuOptions)) { return; }

        if (!TargetWithinRange(MenuSystem.DistanceToTarget(actorID))) { return; }

        // Check for AttackComponent

        MenuSystem.AddOption(actorID, RPGGameEvent.Menu_AttackMelee);
    }

    public static bool TargetWithinRange(float distance)
    {
        return distance <= DistanceTargetable;
    }

    public class DamageTaken 
    {
        public int   EntityID;
        public int   AttackerID;
        public float Damage;
    }
    public class Broken 
    {
        public int EntityID;
    }
}
