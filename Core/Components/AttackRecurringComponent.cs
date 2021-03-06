﻿using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;

[InspectorComponent]
public class AttackRecurringComponent : IEntityComponent
{
    public string Description;
    public DieRollData Damage;
    public AttackSystem.DamageTypes DamageType;

    public void InitComponent(IAttributeTable attributeTable)
    {
    }
}