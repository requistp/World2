using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using UnityEngine;

[GameSystem]
public class MovementSystem : GameSystem
{
    public override void Init(IAttributeTable configuration) 
    {
    }

    public override void LateUpdate(float dt) 
    {
        foreach (var entityID in this.EntityManager.EntitiesWithComponent(typeof(MovementComponent)))
        {
            var form = this.EntityManager.GetComponent<FormComponent>(entityID);
            if (form != null)
            {
                var movement = this.EntityManager.GetComponent<MovementComponent>(entityID);
                if (movement != null)
                {
                    form.TranslatePosition(movement.Velocity * GetSpeed(entityID) * Time.deltaTime);
                    form.SetRotationToNext();
                    form.SetCameraRotationToNext();
                    form.SetJumpForce();
                }
            }
        }
    }

    public float GetSpeed(int entityID) 
    {
        // If crouching, use crouching speed
        var crouch = this.EntityManager.GetComponent<CrouchComponent>(entityID);
        if (crouch != null && crouch.Crouching)
        {
            return crouch.Speed;
        }

        // If running, use run speed
        var run = this.EntityManager.GetComponent<RunComponent>(entityID);
        if (run != null && run.Running)
        {
            return run.Speed;
        }

        // Otherwise use normal speed
        var movement = this.EntityManager.GetComponent<MovementComponent>(entityID);
        if (movement != null)
        {
            return movement.Speed;
        }

        // No Speed
        return 0.0f;
    }
}
