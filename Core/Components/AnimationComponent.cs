using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;
using System.Collections.Generic;

[InspectorComponent]
public class AnimationComponent : IEntityComponent
{
    public List<AnimationSystem.AnimationData> Animations;

    public void InitComponent(IAttributeTable attributeTable) 
    {
        Animations = new List<AnimationSystem.AnimationData>();
    }

}