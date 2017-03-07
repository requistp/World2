using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using UnityEngine;

[GameSystem]
public class ActionSystem : GameSystem
{
    private delegate void Delegate_CallOnActionCompleted(ActionCompletedData data);
    private Delegate_CallOnActionCompleted CallOnActionCompleted;

    public override void Init(IAttributeTable configuration)
    {
        EventManager.RegisterListener(RPGGameEvent.FallLanded, OnFallLanded);
        EventManager.RegisterListener(RPGGameEvent.FallStarted, OnFallStarted);
        EventManager.RegisterListener(RPGGameEvent.JumpStarted, OnJumpStarted);
        EventManager.RegisterListener(RPGGameEvent.Menu_Close, OnMenuClose);
        EventManager.RegisterListener(RPGGameEvent.Menu_DisarmTrap, OnMenuDisarmTrap);
        EventManager.RegisterListener(RPGGameEvent.Menu_Open, OnMenuOpen);
        EventManager.RegisterListener(RPGGameEvent.Menu_PickLock, OnMenuPickLock);
        EventManager.RegisterListener(RPGGameEvent.Menu_UseKey, OnMenuUseKey);
    }

    public override void Update(float dt) 
    {
        base.Update(dt);

        foreach (var e in this.EntityManager.EntitiesWithComponent(typeof(ActionComponent)))
        {
            UpdateTimers(e, dt);
        }
    }

    private void OnFallLanded(GameEvent e) 
    {
        var data = (FallingSystem.FallData)e.EventData;
        
        UntimedActionCompleted(data.EntityID, RPGGameEvent.FallStarted);
    }
    private void OnFallStarted(GameEvent e) 
    {
        var data = (FallingSystem.FallData)e.EventData;

        UntimedActionStarted(data.EntityID, RPGGameEvent.FallStarted);
    }
    private void OnJumpStarted(GameEvent e) 
    {
        var data = (JumpSystem.JumpData)e.EventData;

        UntimedActionStarted(data.EntityID, RPGGameEvent.JumpStarted);
    }
    private void OnMenuClose(GameEvent e) 
    {
        var data = (MenuSystem.MenuOptionSelected)e.EventData;

        TimedActionStarted(data.ActorID, RPGGameEvent.Menu_Close, OpenSystem.ActionTimeToOpen(data.ItemID), true, data.ItemID);
    }
    private void OnMenuDisarmTrap(GameEvent e) 
    {
        var data = (MenuSystem.MenuOptionSelected)e.EventData;

        TimedActionStarted(data.ActorID, RPGGameEvent.Menu_DisarmTrap, TrapSystem.ActionTimeToDisarmTrap(data.ItemID), true, data.ItemID);
    }
    private void OnMenuOpen(GameEvent e) 
    {
        var data = (MenuSystem.MenuOptionSelected)e.EventData;

        TimedActionStarted(data.ActorID, RPGGameEvent.Menu_Open, OpenSystem.ActionTimeToOpen(data.ItemID), true, data.ItemID);
    }
    private void OnMenuPickLock(GameEvent e) 
    {
        var data = (MenuSystem.MenuOptionSelected)e.EventData;

        TimedActionStarted(data.ActorID, RPGGameEvent.Menu_PickLock, LockSystem.ActionTimeToPickLock(data.ItemID), true, data.ItemID);
    }
    private void OnMenuUseKey(GameEvent e) 
    {
        var data = (MenuSystem.MenuOptionSelected)e.EventData;

        TimedActionStarted(data.ActorID, RPGGameEvent.Menu_UseKey, LockSystem.ActionTimeToUseKey(data.ItemID), true, data.ItemID);
    }

    private void TimedActionCompleted(int entityID, ActionComponent action) 
    {
        MenuSystem.ClearOptions(entityID);

        CallOnActionCompleted = ActionCompletedEvent(action.ActionType);
        if (CallOnActionCompleted != null)
        {
            CallOnActionCompleted.Invoke(new ActionCompletedData() { ActorID = entityID, TargetID = action.TargetID, ActionType = action.ActionType });
        }
    }
    private void TimedActionStarted(int entityID, RPGGameEvent actionType, float duration, bool isTimed, int targetID) 
    {
        var action = this.EntityManager.GetComponent<ActionComponent>(entityID);
        if (action == null) { return; }

        action.ActionType = actionType;
        action.Duration = duration;
        action.IsTimed = isTimed;
        action.TargetID = targetID;

        MenuSystem.SetMenuToBusy(entityID);

        this.EventManager.QueueEvent(RPGGameEvent.ActionStarted, new ActionStartedData() { ActorID = entityID, TargetID = action.TargetID, ActionType = action.ActionType });
    }
    private void UntimedActionCompleted(int entityID, RPGGameEvent actionType) 
    {
        MenuSystem.ClearOptions(entityID);

        var action = this.EntityManager.GetComponent<ActionComponent>(entityID);
        if (action != null && action.ActionType == RPGGameEvent.FallStarted)
        {
            action.ActionType = RPGGameEvent.None;
        }
    }
    private void UntimedActionStarted(int entityID, RPGGameEvent actionType) 
    {
        var action = this.EntityManager.GetComponent<ActionComponent>(entityID);
        if (action != null) { return; }

        action.ActionType = actionType;
        action.Duration = 0.0f;
        action.IsTimed = false;
        action.TargetID = -1;

        MenuSystem.SetMenuToBusy(entityID);
    }
    private void UpdateTimers(int entityID, float dt) 
    {
        var action = this.EntityManager.GetComponent<ActionComponent>(entityID);
        if (action.ActionType == RPGGameEvent.Menu_None || !action.IsTimed) { return; }

        action.Duration -= dt;

        if (action.Duration <= 0) 
        {
            TimedActionCompleted(entityID, action);
            action.Clear();
        }
    }

    public static RPGGameEvent CurrentAction(int entityID) 
    {
        var action = GameBehavior.thisGame.EntityManager.GetComponent<ActionComponent>(entityID);

        return (action == null) ? RPGGameEvent.None : action.ActionType;
    }
    public static bool IsBusy(int entityID) 
    {
        var action = GameBehavior.thisGame.EntityManager.GetComponent<ActionComponent>(entityID);

        return (action == null) ? false : action.IsBusy;
    }

    private static Delegate_CallOnActionCompleted ActionCompletedEvent(RPGGameEvent option) 
    {
        switch (option)
        {
            case RPGGameEvent.Menu_DisarmTrap:
                return TrapSystem.OnDisarmTrap;
            case RPGGameEvent.Menu_Close:
                return OpenSystem.ToggleOpen;
            case RPGGameEvent.Menu_Open:
                return OpenSystem.ToggleOpen;
            case RPGGameEvent.Menu_PickLock:
                return LockSystem.OnPickLock;
            case RPGGameEvent.Menu_UseKey:
                return LockSystem.OnUseKey;
            default:
                return null;
        }
    }

    public class ActionCompletedData
    {
        public int ActorID;
        public int TargetID;
        public RPGGameEvent ActionType;
    }
    public class ActionStartedData
    {
        public int ActorID;
        public int TargetID;
        public RPGGameEvent ActionType;
    }
}

