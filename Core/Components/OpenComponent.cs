using Slash.Collections.AttributeTables;
using Slash.ECS.Components;

public class OpenComponent : IEntityComponent
{
    public float ActionTimeToClose;
    public float ActionTimeToOpen;
    public bool  Open;

    public void InitComponent(IAttributeTable attributeTable)
    {
        ActionTimeToClose = 0.5f;
        ActionTimeToOpen = 1.0f;
        Open = false;
    }
}