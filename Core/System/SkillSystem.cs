using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using Slash.ECS.Inspector.Attributes;
using UnityEngine;

[GameSystem]
public class SkillSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        this.EventManager.RegisterListener(RPGGameEvent.PickedLock, OnPickedLock);
        this.EventManager.RegisterListener(RPGGameEvent.TrapDetection, OnTrapDetection);
        this.EventManager.RegisterListener(RPGGameEvent.TrapDetonated, OnTrapDetonated);
        this.EventManager.RegisterListener(RPGGameEvent.TrapDisarmAttempt, OnTrapDisarmAttempt);
    }

    private void OnPickedLock(GameEvent e)
    {
        var data = (LockSystem.PickedLockData)e.EventData;

        var skill = this.EntityManager.GetComponent<SkillComponent>(data.ActorID);
        if (skill == null) { return; }

        if (data.Success)
        {
            skill.PickLockAttempts_Success++;
        }
        else
        {
            skill.PickLockAttempts_Failed++;
        }
    }
    private void OnTrapDetection(GameEvent e) 
    {
        var data = (TrapSystem.TrapDetectionData)e.EventData;

        var skill = this.EntityManager.GetComponent<SkillComponent>(data.ActorID);
        if (skill == null) { return; }

        if (data.Detected)
        {
            skill.TrapDetections_Success++;
        }
        else
        {
            skill.TrapDetections_Failed++;
        }
    }
    private void OnTrapDetonated(GameEvent e) 
    {
        var data = (TrapSystem.TrapDetonationData)e.EventData;

        var skill = this.EntityManager.GetComponent<SkillComponent>(data.ActorID);
        if (skill == null) { return; }

        skill.TrapDetonations++;
    }
    private void OnTrapDisarmAttempt(GameEvent e) 
    {
        var data = (TrapSystem.TrapDisarmData)e.EventData;

        var skill = this.EntityManager.GetComponent<SkillComponent>(data.ActorID);
        if (skill == null) { return; }

        if (data.Disarmed)
        {
            skill.TrapDisarms_Success++;
        }
        else
        {
            skill.TrapDisarms_Failed++;
        }
    }

    //public static int 
}
