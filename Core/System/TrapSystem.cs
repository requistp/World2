using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using Slash.ECS.Inspector.Attributes;
using System.Collections.Generic;
using UnityEngine;

[GameSystem]
public class TrapSystem : GameSystem
{
    private const float DistanceDisarmable = 8.5f;
    private static readonly List<RPGGameEvent> AllMenuOptions = new List<RPGGameEvent>() { RPGGameEvent.Menu_TrapDetected, RPGGameEvent.Menu_DisarmTrap };

    public override void Init(IAttributeTable configuration)
    {
        this.EventManager.RegisterListener(RPGGameEvent.Input_Mouse_Hit, OnMouseHit);
        this.EventManager.RegisterListener(RPGGameEvent.Unity_OnTrapDetectionRangeEntered, OnTrapDetectionRangeEntered);
        this.EventManager.RegisterListener(RPGGameEvent.Opened, OnOpened);
    }

    private void OnMouseHit(GameEvent e) 
    {
        var data = (InputMouseBehaviour.MouseData)e.EventData;

        SetMenuOptions(data.EntityID, data.HitEntityID);
    }
    private void OnOpened(GameEvent e) 
    {
        var data = (OpenSystem.ToggleOpenData)e.EventData;

        DetonateTrap(data.ActorID, data.ItemID);

        GameBehavior.thisGame.EventManager.QueueEvent(RPGGameEvent.TrapDetonated, new TrapDetonationData() { ActorID = data.ActorID, ItemID = data.ItemID });
    }
    private void OnTrapDetectionRangeEntered(GameEvent e) 
    {
        var data = (EntityBehaviour.Unity_OnTriggerEnterData)e.EventData;

        var trap = this.EntityManager.GetComponent<TrapComponent>(data.ItemID);
        if (trap == null) { return; }

        var skill = this.EntityManager.GetComponent<SkillComponent>(data.ActorID);
        if (skill == null) { return; }

        // If if already tried to detect, then exit
        if (trap.DetectionAttempted(data.ActorID)) { return; }

        //Try detecting trap
        bool detected = DetectTrapResult(skill, trap);
        if (detected)
        {
            trap.AddDetectedBy(data.ActorID);
        }
        trap.AddDetectionAttempt(data.ActorID);

        this.EventManager.QueueEvent(RPGGameEvent.TrapDetection, new TrapDetectionData() { ActorID = data.ActorID, ItemID = data.ItemID, Detected = detected });
    }
    private void DetonateTrap(int actorID, int itemID) 
    {
        var trap = this.EntityManager.GetComponent<TrapComponent>(itemID);
        if (trap == null) { return; }

        trap.Effect.Location = trap.FormParent.Position_Center; // Location is unset until detonation because a trap on a chest can move

        EffectSystem.CreateEffectEntity(this.EntityManager, trap.Effect);
        
        RemoveTrap(itemID, trap);
    }
    private bool DetectTrapResult(SkillComponent skill, TrapComponent trap) 
    {
        bool detected = skill.TrapDetectionLevel >= trap.DetectionLevel;

        return detected;
    }
    private void SetMenuOptions(int actorID, int itemID) 
    {
        if (!MenuSystem.RemoveOptions(actorID, AllMenuOptions)) { return; }

        if (ActionSystem.IsBusy(actorID)) { return; }

        if (!ItemWithinRange(MenuSystem.DistanceToTarget(actorID))) { return; }

        var trap = this.EntityManager.GetComponent<TrapComponent>(itemID); if (trap == null) { return; }
        
        // Not Detected
        if (!trap.IsDetectedBy(actorID)) { return; }

        // No disarm skill or already tried to disarm?
        var skill = this.EntityManager.GetComponent<SkillComponent>(actorID);
        if (skill == null || skill.TrapDisarmLevel == 0 || trap.DisarmAttempted(actorID))
        {
            MenuSystem.AddOption(actorID, RPGGameEvent.Menu_TrapDetected);
        }
        else // Can try disarming
        {            
            MenuSystem.AddOption(actorID, RPGGameEvent.Menu_DisarmTrap);
        }
    }

    public static float ActionTimeToDisarmTrap(int entityID) 
    {
        var trap = GameBehavior.thisGame.EntityManager.GetComponent<TrapComponent>(entityID);

        return (trap == null) ? 0.0f : trap.ActionTimeToDisarmTrap;
    }
    public static void OnDisarmTrap(ActionSystem.ActionCompletedData data) 
    {
        var trap = GameBehavior.thisGame.EntityManager.GetComponent<TrapComponent>(data.TargetID);
        if (trap == null) { return; }

        var skill = GameBehavior.thisGame.EntityManager.GetComponent<SkillComponent>(data.ActorID);
        if (skill == null) { return; }

        // Disarmed?
        bool disarmed = DisarmTrapResult(skill, trap);
        if (disarmed)
        {
            RemoveTrap(data.TargetID, trap);
        }
        else
        {
            trap.AddDisarmAttemptedBy(data.ActorID);
        }
        
        // Event
        GameBehavior.thisGame.EventManager.QueueEvent(RPGGameEvent.TrapDisarmAttempt, new TrapDisarmData() { ActorID = data.ActorID, ItemID = data.TargetID, Disarmed = disarmed });
    }

    private static bool DisarmTrapResult(SkillComponent skill, TrapComponent trap) 
    {
        bool success = skill.TrapDisarmLevel >= trap.DisarmLevel;

        return success;
    }
    private static bool ItemWithinRange(float distance) 
    {
        return distance <= DistanceDisarmable;
    }
    private static void RemoveTrap(int itemID, TrapComponent trap) 
    {
        trap.FormParent.DestroyChild("Trap");
        GameBehavior.thisGame.EntityManager.RemoveComponent(itemID, typeof(TrapComponent));
    }

    public class TrapDetectionData 
    {
        public int ActorID;
        public int ItemID;
        public bool Detected;
    }
    public class TrapDetonationData
    {
        public int ActorID;
        public int ItemID;
    }
    public class TrapDisarmData 
    {
        public int  ActorID;
        public int  ItemID;
        public bool Disarmed;
    }
}
