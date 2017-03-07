using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;
using System.Collections.Generic;

[InspectorComponent]
public class ActionComponent : IEntityComponent
{
    public RPGGameEvent ActionType;
    public float Duration;
    public bool IsTimed;
    public int TargetID;

    public bool IsBusy 
    {
        get
        {
            return ActionType != RPGGameEvent.None;
        }
    }

    public void InitComponent(IAttributeTable attributeTable) 
    {
        Clear();
    }

    public void Clear() 
    {
        ActionType = RPGGameEvent.None;
        Duration = 0.0f;
        IsTimed = false;
        TargetID = -1;
    }
}