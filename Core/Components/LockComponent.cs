using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;
using System.Collections.Generic;

[InspectorComponent]
public class LockComponent : IEntityComponent
{
    public const string AttributeLocked = "LockComponent.Locked";
    public const string AttributeKeyEntityID = "LockComponent.KeyEntityID";
    public const string AttributeMagicUnlockLevel = "LockComponent.MagicUnlockLevel";
    public const string AttributePickSkillLevel = "LockComponent.PickSkillLevel";

    private List<int> PickAttempts;

    public float ActionTimeToPickLock;
    public float ActionTimeToUseKey;
    public bool HasKey 
    {
        get
        {
            return (KeyEntityID > 0);
        }
    }
    public bool IsPickable 
    {
        get
        {
            return PickSkillLevel >= 0;
        }
    }
    public int  KeyEntityID;
    public int  MagicUnlockLevel; // -1 means not magic unlockable
    public int  PickSkillLevel; // -1 means not pickable

    public void InitComponent(IAttributeTable attributeTable) 
    {
        if (attributeTable != null)
        {
            this.KeyEntityID = attributeTable.GetIntOrDefault(AttributeKeyEntityID, 0);
            this.MagicUnlockLevel = attributeTable.GetIntOrDefault(AttributeMagicUnlockLevel, 1);
            this.PickSkillLevel = attributeTable.GetIntOrDefault(AttributePickSkillLevel, 1);
        }
        else
        {
            ActionTimeToPickLock = 2.0f;
            ActionTimeToUseKey = 1.5f;
            this.KeyEntityID = -1;
            this.MagicUnlockLevel = 1;
            this.PickSkillLevel = 1;
            this.PickAttempts = new List<int>();
        }
    }
    public void InitComponent(int keyEntityID) 
    {
        this.KeyEntityID = keyEntityID;
    }

    public void AddPickLockAttempt(int actorID) 
    {
        if (!PickAttempts.Contains(actorID))
        {
            PickAttempts.Add(actorID);
        }
    }
    public bool PickLockAttempted(int actorID) 
    {
        return PickAttempts.Contains(actorID);
    }
}