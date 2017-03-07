using Slash.Application.Games;
using Slash.ECS.Inspector.Attributes;
using Slash.Unity.Common.ECS;
using UnityEngine;

public class EntityBehaviour : Slash.Unity.Common.ECS.EntityBehaviour
{
    public void Init(int entityID, Game game)
    {
        base.EntityId = entityID;
        base.Game = game;
    }

    private void OnTriggerEnter(Collider collider) 
    {
        var elb = collider.transform.root.GetComponent<EntityBehaviour>() as EntityBehaviour;
        if (elb == null) { return; }

        // Trap Detection Range
        if (collider.tag == "TrapDetectionRange")
        {
            this.Game.EventManager.QueueEvent(RPGGameEvent.Unity_OnTrapDetectionRangeEntered, new Unity_OnTriggerEnterData() { ActorID = base.EntityId, ItemID = elb.EntityId });
        }

        // So each specific trigger type is checked for here and translated into the specific Slash type game event
    }
    private void OnTriggerStay(Collider collider) 
    {
        var elb = collider.GetComponentInParent<EntityBehaviour>() as EntityBehaviour;
        if (elb == null) { return; }

        // Area Of Effect
        if (collider.tag == "AreaOfEffect")
        {
            this.Game.EventManager.QueueEvent(RPGGameEvent.Unity_OnAreaOfEffectStay, new Unity_OnTriggerStayData() { EntityID_Target = base.EntityId, EntityID_AOE = elb.EntityId, DeltaTime = Time.deltaTime });
        }

        // So each specific trigger type is checked for here and translated into the specific Slash type game event
    }

    public class Unity_OnTriggerEnterData 
    {
        public int ActorID;
        public int ItemID;
    }
    public class Unity_OnTriggerStayData 
    {
        public int   EntityID_Target;
        public int   EntityID_AOE;
        public float DeltaTime;
    }
}

