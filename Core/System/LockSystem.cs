using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using Slash.SystemExt.Utils;
using System.Collections.Generic;
using UnityEngine;

[GameSystem]
public class LockSystem : GameSystem
{
    private const float DistanceUnlockable = 8.5f;
    private static readonly List<RPGGameEvent> AllMenuOptions = new List<RPGGameEvent>() { RPGGameEvent.Menu_UseKey, RPGGameEvent.Menu_Locked, RPGGameEvent.Menu_PickLock };

    public override void Init(IAttributeTable configuration)
    {
        this.EventManager.RegisterListener(RPGGameEvent.Input_Mouse_Hit, OnMouseHit);
    }

    private void OnMouseHit(GameEvent e) 
    {
        var data = (InputMouseBehaviour.MouseData)e.EventData;

        SetMenuOptions(data.EntityID, data.HitEntityID);
    }
    private void SetMenuOptions(int actorID, int itemID)  
    {
        if (!MenuSystem.RemoveOptions(actorID, AllMenuOptions)) { return; }

        if (ActionSystem.IsBusy(actorID)) { return; }

        if (!ItemWithinRange(MenuSystem.DistanceToTarget(actorID))) { return; }
       
        var lockComp = this.EntityManager.GetComponent<LockComponent>(itemID); if (lockComp == null) { return; }

        bool unlockOptionExists = false;
        if (lockComp.IsPickable && !lockComp.PickLockAttempted(actorID)) // IF lock is pickable AND actor has not attempted to pick this lock...
        {            
            var skill = this.EntityManager.GetComponent<SkillComponent>(actorID);
            if (skill != null && skill.PickLockLevel >= 0) // ...AND actor can pick locks THEN add option
            {
                MenuSystem.AddOption(actorID, RPGGameEvent.Menu_PickLock);
                unlockOptionExists = true;
            }
        }
        if (lockComp.HasKey && ActorHasKey(actorID, lockComp.KeyEntityID))
        {
            MenuSystem.AddOption(actorID, RPGGameEvent.Menu_UseKey);
            unlockOptionExists = true;
        }
        if (!unlockOptionExists)
        {
            MenuSystem.AddOption(actorID, RPGGameEvent.Menu_Locked);
        }
    }

    private static bool ItemWithinRange(float distance) 
    {
        return distance <= DistanceUnlockable;
    }
    private static bool PickLockResult(SkillComponent skillComp, LockComponent lockComp) 
    {
        if (!lockComp.IsPickable) { return false; }

        bool success = skillComp.PickLockLevel >= lockComp.PickSkillLevel;

        // could make more complicated

        return success; 
    }

    public static float ActionTimeToPickLock(int entityID) 
    {
        var lockComp = GameBehavior.thisGame.EntityManager.GetComponent<LockComponent>(entityID);

        return (lockComp == null) ? 0.0f : lockComp.ActionTimeToPickLock;
    }
    public static float ActionTimeToUseKey(int entityID) 
    {
        var lockComp = GameBehavior.thisGame.EntityManager.GetComponent<LockComponent>(entityID);

        return (lockComp == null) ? 0.0f : lockComp.ActionTimeToUseKey;
    }
    public static bool IsLocked(int entityID) 
    {
        var lockComp = GameBehavior.thisGame.EntityManager.GetComponent<LockComponent>(entityID);

        return lockComp != null;
    }
    public static void OnPickLock(ActionSystem.ActionCompletedData data) 
    {
        var lockComp = GameBehavior.thisGame.EntityManager.GetComponent<LockComponent>(data.TargetID);
        if (lockComp == null) { return; }

        var skill = GameBehavior.thisGame.EntityManager.GetComponent<SkillComponent>(data.ActorID);
        if (skill == null) { return; }

        bool success = PickLockResult(skill, lockComp);
        if (success)
        {
            GameBehavior.thisGame.EntityManager.RemoveComponent(data.TargetID, typeof(LockComponent));
        }
        else
        {
            lockComp.AddPickLockAttempt(data.ActorID);
        }

        GameBehavior.thisGame.EventManager.QueueEvent(RPGGameEvent.PickedLock, new PickedLockData() { ActorID = data.ActorID, ItemID = data.TargetID, Success = success });
    }
    public static void OnUseKey(ActionSystem.ActionCompletedData data) 
    {
        var lockComp = GameBehavior.thisGame.EntityManager.GetComponent<LockComponent>(data.TargetID);
        if (lockComp == null) { return; }

        // Need to get key from inventory here (and maybe remove it?)
        int keyID = lockComp.KeyEntityID;

        GameBehavior.thisGame.EntityManager.RemoveComponent(data.TargetID, typeof(LockComponent));

        GameBehavior.thisGame.EventManager.QueueEvent(RPGGameEvent.UsedKey, new UsedKeyData() { ActorID = data.ActorID, ItemID = data.TargetID, KeyID = keyID });
    }

    public class PickedLockData
    {
        public int ActorID;
        public int ItemID;
        public bool Success;
    }
    public class UsedKeyData
    {
        public int ActorID;
        public int ItemID;
        public int KeyID;
    }

    private bool ActorHasKey(int actorid, int keyID) // Obviously, this needs to check inventory
    {
        return true;
    }
}



