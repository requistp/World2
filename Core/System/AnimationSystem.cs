using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using System.Collections.Generic;
using UnityEngine;

[GameSystem]
public class AnimationSystem : GameSystem
{
    private static List<AnimationData> ActiveAnimations;

    public override void Init(IAttributeTable configuration)
    {
        ActiveAnimations = new List<AnimationData>();

        this.EventManager.RegisterListener(RPGGameEvent.ActionStarted, OnActionStarted);
    }

    public override void Update(float dt) 
    {
        CheckActiveAnimations();
    }

    public static void RemoveAll(int entityID) 
    {
        ActiveAnimations.RemoveAll(x => x.EntityID == entityID);
    }

    private void CheckActiveAnimations() 
    {
        var r = new List<AnimationData>();

        foreach(var d in ActiveAnimations)
        {
            if (d.theAnimation == null)
            {
                ActiveAnimations.Remove(d);
            }
            else if (!d.theAnimation.isPlaying)
            {
                if (d.DisablePhysics)
                {
                    var formComponent = this.EntityManager.GetComponent<FormComponent>(d.EntityID);
                    if (formComponent != null)
                    {
                        formComponent.PhysicsEnabled = true;
                    }
                }

                r.Add(d);

                this.EventManager.QueueEvent(RPGGameEvent.AnimationEnded, d);
            }
        }

        ActiveAnimations.RemoveAll(x => r.Contains(x));
    }
    private void Play(int entityID, RPGGameEvent actionStarted) 
    {
        var animation = this.EntityManager.GetComponent<AnimationComponent>(entityID);
        if (animation == null) { return; }

        var form = this.EntityManager.GetComponent<FormComponent>(entityID);
        if (form == null) { return; }

        var animationData = animation.Animations.Find(x => x.TriggerEvent == actionStarted);
        if (animationData == null) { return; }

        if (animationData.DisablePhysics)
        {
            form.PhysicsEnabled = false;
        }

        animationData.theAnimation = form.PlayAnimation(animationData.Name, animationData.Sound);
        if (animationData.theAnimation != null)
        {
            ActiveAnimations.Add(animationData);
            this.EventManager.QueueEvent(RPGGameEvent.AnimationStarted, animationData);
        }
    }
    private void OnActionStarted(GameEvent e) 
    {
        var data = (ActionSystem.ActionStartedData)e.EventData;

        Play(data.ActorID, data.ActionType);
        Play(data.TargetID, data.ActionType);
    }

    public class AnimationData 
    {
        public int          EntityID;
        public Animation    theAnimation;
        public bool         DisablePhysics;
        public RPGGameEvent TriggerEvent;
        public string       Name;
        public string       Sound;
    }
}
