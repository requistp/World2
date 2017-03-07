using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using UnityEngine;

[GameSystem]
public class UnityGORemovalSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        this.EventManager.RegisterListener(FrameworkEvent.EntityRemoved, OnEntityRemoved);
        this.EventManager.RegisterListener(FrameworkEvent.ComponentRemoved, OnComponentRemoved);
    }

    private void OnComponentRemoved(GameEvent e)
    {
        var data = (EntityComponentData)e.EventData;

        var form = data.Component as FormComponent;
        if (form != null)
        {
            AnimationSystem.RemoveAll(data.EntityId);
            form.DestroyForm();
        }
    }
    private void OnEntityRemoved(GameEvent e)
    {
        var form = this.EntityManager.GetComponent<FormComponent>((int)e.EventData);
        if (form != null)
        {
            form.DestroyForm();
        }
    }
}
