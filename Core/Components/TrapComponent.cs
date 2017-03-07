using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using System.Collections.Generic;

public class TrapComponent : IEntityComponent
{
    public const string PrefabName_TrapDetectionRange = "TrapDetectionRange";

    private List<int> _DetectedBy;
    private List<int> _DetectionAttemptedBy;
    private List<int> _DisarmAttemptedBy;

    public float                   ActionTimeToDisarmTrap;
    public int                     DetectionLevel;
    public float                   DetectionRange;
    public int                     DisarmLevel;
    public FormComponent           FormParent;
    public EffectSystem.EffectData Effect;

    public void InitComponent(IAttributeTable attributeTable) 
    {
    }
    public void InitComponent(FormComponent formParent, EffectSystem.EffectData effect) 
    {
        if (formParent == null) { return; }

        _DetectedBy = new List<int>();
        _DetectionAttemptedBy = new List<int>();
        _DisarmAttemptedBy = new List<int>();

        ActionTimeToDisarmTrap = 3.0f;
        DetectionLevel = 2;
        DetectionRange = MenuSystem.DistanceItemActionable;
        DisarmLevel = 2;

        FormParent = formParent;
        Effect = effect;

        ConfigureTrap(this, formParent);
    }
    private void ConfigureTrap(TrapComponent trap, FormComponent formParent) 
    {
        // Configure FormComponent
        var trapGO = formParent.AddChild(PrefabName_TrapDetectionRange);
        if (trapGO != null)
        {
            formParent.SetSphereColliderRadius(trapGO, trap.DetectionRange);
        }

        // Configure more stuff here...
    }

    public void AddDetectionAttempt(int actorID) 
    {
        if (!_DetectionAttemptedBy.Contains(actorID))
        {
            _DetectionAttemptedBy.Add(actorID);
        }
    }
    public void AddDetectedBy(int actorID) 
    {
        if (!_DetectedBy.Contains(actorID))
        {
            _DetectedBy.Add(actorID);
        }
    }
    public void AddDisarmAttemptedBy(int actorID) 
    {
        if (!_DisarmAttemptedBy.Contains(actorID))
        {
            _DisarmAttemptedBy.Add(actorID);
        }
    }
    public bool DetectionAttempted(int actorID) 
    {
        return _DetectionAttemptedBy.Contains(actorID);
    }
    public bool DisarmAttempted(int actorID) 
    {
        return _DisarmAttemptedBy.Contains(actorID);
    }
    public bool IsDetectedBy(int actorID) 
    {
        return _DetectedBy.Contains(actorID);
    }
}