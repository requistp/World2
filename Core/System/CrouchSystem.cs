using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using Slash.ECS.Inspector.Attributes;
using UnityEngine;

[GameSystem]
[InspectorType]
public class CrouchSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        this.EventManager.RegisterListener(RPGGameEvent.Input_Move, OnMoveInput);
    }

    private void OnMoveInput(GameEvent e)
    {
        var data = (InputMoveBehaviour.MoveData)e.EventData;

        // Crouching was toggled?
        if (!data.CrouchToggled) { return; }

        // Busy?
        if (ActionSystem.IsBusy(data.EntityID)) { return; }

        ToggleCrouch(data.EntityID);
    }

    private void ToggleCrouch(int entityID)
    {
        var crouch = this.EntityManager.GetComponent<CrouchComponent>(entityID);
        if (crouch == null) { return; }

        crouch.Crouching = !crouch.Crouching;
    }
}
