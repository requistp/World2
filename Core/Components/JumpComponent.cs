using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;

[InspectorComponent]
public class JumpComponent : IEntityComponent
{
    //public const string AttributeVelocity = "JumpComponent.Velocity";

    public const float DefaultVelocity = 50000.0f;

    public bool  Jumping;
    public float Velocity;

    public void InitComponent(IAttributeTable attributeTable)
    {
        Jumping = false;
        Velocity = DefaultVelocity;
    }
}
