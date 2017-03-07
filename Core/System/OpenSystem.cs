using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using System.Collections.Generic;
using UnityEngine;

[GameSystem]
public class OpenSystem : GameSystem
{
    private const float DistanceOpenable = 8.5f;
    private static readonly List<RPGGameEvent> AllMenuOptions = new List<RPGGameEvent>() { RPGGameEvent.Menu_Close, RPGGameEvent.Menu_Open };
    
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

        if (LockSystem.IsLocked(itemID)) { return; }

        MenuSystem.AddOption(actorID, IsOpen(itemID) ? RPGGameEvent.Menu_Close : RPGGameEvent.Menu_Open);
    }

    public static float ActionTimeToClose(int entityID) 
    {
        var open = GameBehavior.thisGame.EntityManager.GetComponent<OpenComponent>(entityID);

        return (open == null) ? 0.0f : open.ActionTimeToClose;
    }
    public static float ActionTimeToOpen(int entityID) 
    {
        var open = GameBehavior.thisGame.EntityManager.GetComponent<OpenComponent>(entityID);

        return (open == null) ? 0.0f : open.ActionTimeToOpen;
    }
    public static bool IsOpen(int entityID) 
    {
        var open = GameBehavior.thisGame.EntityManager.GetComponent<OpenComponent>(entityID);

        return (open == null) ? false : open.Open;
    }
    public static void ToggleOpen(ActionSystem.ActionCompletedData data) 
    {
        var open = GameBehavior.thisGame.EntityManager.GetComponent<OpenComponent>(data.TargetID);
        if (open != null)
        {
            open.Open = !open.Open;
        }

        GameBehavior.thisGame.EventManager.QueueEvent((data.ActionType == RPGGameEvent.Menu_Open) ? RPGGameEvent.Opened : RPGGameEvent.Closed, new ToggleOpenData() { ActorID = data.ActorID, ItemID = data.TargetID });
    }

    private static bool ItemWithinRange(float distance) 
    {
        return distance <= DistanceOpenable;
    }

    public class ToggleOpenData
    {
        public int ActorID;
        public int ItemID;
    }
}
