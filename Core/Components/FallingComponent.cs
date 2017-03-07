using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;

[InspectorComponent]
public class FallingComponent : IEntityComponent
{
    public float HeightStart;

    public void InitComponent(IAttributeTable attributeTable)
    {
    }

    public void InitComponent(float heightStart)
    {
        HeightStart = heightStart;
    }
}