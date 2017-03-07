using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;

[InspectorComponent]
public class HealthComponent : IEntityComponent
{
    public float      Damage;
    public bool       DamageDisabled 
    {
        get
        {
            return HP_Current <= 0;
        }
    }
    public DeathTypes DeathType;
    public float      HP_Current 
    {
        get
        {
            return HP_Max - Damage;
        }
    }
    public float      HP_Max;

    public void InitComponent(IAttributeTable attributeTable)
    {
    }

    public class DamageTaken
    {
        public int EntityID;
        public int Damage;
    }
    public enum DeathTypes
    {
        Broken,
        LifeEnded
    }
}