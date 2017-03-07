using Slash.Collections.AttributeTables;
using Slash.ECS.Components;

public class DurationComponent : IEntityComponent
{
    public float Duration;
    public bool  TimerActive;
    public float TimeRemaining;

    public void InitComponent(IAttributeTable attributeTable)
    {
    }
    public void InitComponent(float duration, bool timerActive)
    {
        Duration = duration;
        TimerActive = timerActive;
        TimeRemaining = duration;
    }
}