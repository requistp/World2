using UnityEngine;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;
using System;
using System.Collections.Generic;
using Slash.Math.Algebra.Vectors;

[InspectorComponent]
public class MovementComponent : IEntityComponent
{
    public const string AttributeSpeedNormal = "MovementComponent.Speed";

    public const float DefaultSpeed = 5.0f;

    public float   Speed;
    public Vector3 Velocity;

    public void InitComponent(IAttributeTable attributeTable)
    {
        Speed = DefaultSpeed;
    }
}
