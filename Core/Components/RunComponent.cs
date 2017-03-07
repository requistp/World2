using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;

[InspectorComponent]
public class RunComponent : IEntityComponent
{
    public const string AttributeSpeedNormal = "RunComponent.Speed";

    public const float DefaultSpeed = 10.0f;

    public bool  Running;
    public float Speed;

    public void InitComponent(IAttributeTable attributeTable)
    {
        Running = false;
        Speed = DefaultSpeed;
    }
}
