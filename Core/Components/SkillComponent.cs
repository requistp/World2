using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;

[InspectorComponent]
public class SkillComponent : IEntityComponent
{
    // Skills:
    public int PickLockLevel;
    public int TrapDetectionLevel;
    public int TrapDisarmLevel;

    // History:
    public int PickLockAttempts_Failed;
    public int PickLockAttempts_Success;
    public int TrapDetections_Failed;
    public int TrapDetections_Success;
    public int TrapDetonations;
    public int TrapDisarms_Failed;
    public int TrapDisarms_Success;

    public void InitComponent(IAttributeTable attributeTable)
    {
        PickLockAttempts_Failed = 0;
        PickLockAttempts_Success = 0;
        PickLockLevel = 1;
        TrapDetectionLevel = 10;
        TrapDetections_Failed = 0;
        TrapDetections_Success = 0;
        TrapDetonations = 0;
        TrapDisarmLevel = 10;
    }
}