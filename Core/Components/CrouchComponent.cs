using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;

[InspectorComponent]
public class CrouchComponent : IEntityComponent
{
    public const float DefaultScaleFactorMoving = 1.0f;
    public const float DefaultScaleFactorStationary = 1.0f;
    public const float DefaultSpeed = 2.0f;

    public bool  Crouching;
    public float ScaleFactorMoving;
    public float ScaleFactorStationary;
    public float Speed;

    public void InitComponent(IAttributeTable attributeTable)
    {
        Crouching = false;
        ScaleFactorMoving = DefaultScaleFactorMoving;
        ScaleFactorStationary = DefaultScaleFactorStationary;
        Speed = DefaultSpeed;
    }
}