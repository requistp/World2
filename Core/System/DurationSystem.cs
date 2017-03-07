using Slash.Application.Games;
using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;
using Slash.ECS.Inspector.Attributes;
using System.Collections.Generic;
using UnityEngine;

[GameSystem]
public class DurationSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
    }

    public override void Update(float dt)
    {
        List<int> removeList = new List<int>();
        foreach (var id in this.EntityManager.EntitiesWithComponent(typeof(DurationComponent)))
        {
            var duration = this.EntityManager.GetComponent<DurationComponent>(id);
            if (duration != null && duration.TimerActive)
            {
                duration.TimeRemaining -= dt;

                if (duration.TimeRemaining <= 0)
                {
                    removeList.Add(id);
                }
            }
        }

        foreach(var id in removeList)
        {
            this.EntityManager.RemoveEntity(id);
        }
    }
}
