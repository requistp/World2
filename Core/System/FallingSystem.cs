using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using UnityEngine;

[GameSystem]
public class FallingSystem : GameSystem
{
    public const float MaximumStepDistance = 0.333f;

    public override void Update(float dt)
    {
        foreach (var entityID in this.EntityManager.EntitiesWithComponent(typeof(MovementComponent)))
        {
            var form = this.EntityManager.GetComponent<FormComponent>(entityID);
            if (form != null)
            {
                var falling = this.EntityManager.GetComponent<FallingComponent>(entityID);

                // Just started falling?
                if (falling == null && form.Velocity.y < 0 && form.DistanceBelow > MaximumStepDistance)
                {
                    OnFallStarted(entityID, form, falling);
                    continue;
                }

                // Fall landed?
                if (falling != null && form.Velocity.y >= 0)
                {
                    OnFallLanded(new FallData() { EntityID = entityID, HeightStart = falling.HeightStart, HeightEnd = form.Position.y });
                }
            }
        }
    }

    private void OnFallLanded(FallData fallData)
    {
        this.EntityManager.RemoveComponent(fallData.EntityID, typeof(FallingComponent));

        // I should have, short/long falls, short you just get up, longer you take damage and are knocked down

        this.EventManager.QueueEvent(RPGGameEvent.FallLanded, fallData);
    }

    private void OnFallStarted(int entityID, FormComponent formComponent, FallingComponent fallingComponent)
    {
        fallingComponent = new FallingComponent();
        fallingComponent.InitComponent(formComponent.Position.y);
        this.EntityManager.AddComponent(entityID, fallingComponent);

        this.EventManager.QueueEvent(RPGGameEvent.FallStarted, new FallData() { EntityID = entityID, HeightStart = fallingComponent.HeightStart });
    }

    public class FallData
    {   
        public int   EntityID;
        public float HeightStart;
        public float HeightEnd;
    }
}

