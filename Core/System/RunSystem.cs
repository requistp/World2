using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;

[GameSystem]
public class RunSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        this.EventManager.RegisterListener(RPGGameEvent.Input_Move, OnMoveInput);
    }

    private void OnMoveInput(GameEvent e)
    {
        var data = (InputMoveBehaviour.MoveData)e.EventData;

        // Busy?
        if (ActionSystem.IsBusy(data.EntityID)) { return; }

        // Set running
        var run = this.EntityManager.GetComponent<RunComponent>(data.EntityID);
        if (run == null) { return; }

        run.Running = data.Running;
    }
}
