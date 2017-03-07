using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using UnityEngine;

[GameSystem]
public class JumpSystem : GameSystem
{
    public override void Init(IAttributeTable configuration) 
    {
        this.EventManager.RegisterListener(RPGGameEvent.Input_Move, OnMoveInput);
        this.EventManager.RegisterListener(RPGGameEvent.FallLanded, OnFallLanded);
    }

    private void OnMoveInput(GameEvent e) 
    {
        var data = (InputMoveBehaviour.MoveData)e.EventData;

        // Jump action?
        if (!data.Jump) { return; }

        // Busy?
        if (ActionSystem.IsBusy(data.EntityID)) { return; }

        // Jump
        var jump = this.EntityManager.GetComponent<JumpComponent>(data.EntityID);
        if (jump == null || jump.Jumping) { return; }

        // Add Y velocity and raise event
        var form = this.EntityManager.GetComponent<FormComponent>(data.EntityID);
        if (form != null)
        {
            jump.Jumping = true;
            form.NewJumpForce = jump.Velocity;

            this.EventManager.QueueEvent(RPGGameEvent.JumpStarted, new JumpData() { EntityID = data.EntityID });
        }
    }

    private void OnFallLanded(GameEvent e) 
    {
        var data = (FallingSystem.FallData)e.EventData;

        var jump = this.EntityManager.GetComponent<JumpComponent>(data.EntityID);
        if (jump == null) { return; }

        if (jump.Jumping)
        {
            jump.Jumping = false;
            this.EventManager.QueueEvent(RPGGameEvent.JumpLanded, new JumpData() { EntityID = data.EntityID });
        }
    }

    public class JumpData 
    {
        public int EntityID;
    }
}

