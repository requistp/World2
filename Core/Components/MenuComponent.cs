using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;
using System.Collections.Generic;
using UnityEngine;

[InspectorComponent]
public class MenuComponent : IEntityComponent
{
    public SortedList<int,RPGGameEvent> MenuOptions;

    public float BusyDotDuration = 0.1f;
    public float OptionAdditionalRotationY = 180.0f;
    public float OptionRotationRate = 8.0f;
    public float OptionTransparencyDefault = 0.25f;
    public float OptionTransparencyMouseOver = 0.5f;
    public float OptionTransparencySelected = 0.75f;
    public bool  UseCameraForMenuPositioning = true;

    public int          BusyDotsActive;
    public float        BusyDotTimer;
    public float        Distance;
    public int          EntityID_Item;
    public GameObject   MenuGO;
    public bool         MouseButtonPressed;
    public RPGGameEvent SelectedOption;
    public new string   ToString 
    {
        get
        {
            string s = "";

            foreach (var kvp in MenuOptions)
            {
                s += kvp.Value.ToString() + ", ";
            }

            return s;
        }
    }
    
    public void InitComponent(IAttributeTable attributeTable)
    {
    }
    public void InitComponent(int entityID, string prefabName) 
    {
        MenuOptions = new SortedList<int, RPGGameEvent>();

        MenuGO = GameObject.Instantiate(Resources.Load("Prefabs/" + prefabName)) as GameObject;
        
        var eb = MenuGO.GetComponent<EntityBehaviour>() as EntityBehaviour;
        if (eb != null)
        {
            eb.EntityId = entityID;
            eb.Game = GameBehavior.thisGame;
        }

        ClearAll();
    }

    public void AddOption(RPGGameEvent option) 
    {
        int s = RPGGameEventUtils.MenuSortOrder(option);
        if (!MenuOptions.ContainsKey(s))
        {
            MenuOptions.Add(s, option);
        }
    }
    public void ClearAll() 
    {
        MenuOptions = new SortedList<int, RPGGameEvent>();
        
        Distance = -1;
        EntityID_Item = -1;
        MouseButtonPressed = false;
        SelectedOption = RPGGameEvent.None;
    }
    public void ClearOptions() 
    {
        MenuOptions.Clear();
    }
    public bool OptionIsActive(RPGGameEvent option) 
    {
        return MenuOptions.ContainsKey(RPGGameEventUtils.MenuSortOrder(option));
    }
    public void RemoveOption(RPGGameEvent option) 
    {
        MenuOptions.Remove(RPGGameEventUtils.MenuSortOrder(option));
    }
}