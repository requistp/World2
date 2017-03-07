using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

[GameSystem]
public class MenuSystem : GameSystem
{
    public const float DistanceItemActionable = 8.5f;

    private const float MenuOptionSpacing = 1.2f;

    public override void Init(IAttributeTable configuration) 
    {
        this.EventManager.RegisterListener(RPGGameEvent.Input_Mouse, OnMouseUpdate);
        this.EventManager.RegisterListener(FrameworkEvent.ComponentRemoved, OnComponentRemoved);
    }

    public override void Update(float dt) 
    {
        base.Update(dt);

        foreach (var id in this.EntityManager.EntitiesWithComponent(typeof(MenuComponent)))
        {
            var menu = this.EntityManager.GetComponent<MenuComponent>(id);

            if (menu.EntityID_Item <= 0)
            {
                SetMenuActive(menu, false);
            }
            else
            {
                if (ActionSystem.IsBusy(id))
                {
                    SetBusyDots(menu, dt);
                }

                SetMenuOptions(menu);

                SetMenuOptionSpacing(menu);

                SetMenuPosition(id, menu);

                SetMenuActive(menu, true);
            }
        }
    }

    private void OnComponentRemoved(GameEvent e) 
    {
        var data = (EntityComponentData)e.EventData;
        if (data.Component.GetType() != typeof(MenuComponent)) { return; }

        var menu = this.EntityManager.GetComponent<MenuComponent>(data.EntityId);
        if (menu != null)
        {
            GameObject.Destroy(menu.MenuGO);
        }
    }
    private void OnMouseUpdate(GameEvent e) 
    {
        var data = (InputMouseBehaviour.MouseData)e.EventData;

        var menu = this.EntityManager.GetComponent<MenuComponent>(data.EntityID);
        if (menu == null) { return; }

        // Start by assuming nothing was selected
        menu.MouseButtonPressed = false;
        menu.SelectedOption = RPGGameEvent.None;

        if (data.HitMenu)
        {
            menu.MouseButtonPressed = data.MouseButton0;
            menu.SelectedOption = RPGGameEventUtils.GetEnumFromString(data.HitMenuCollider.name);
        }
        else
        {
            menu.Distance = data.HitDistance;
            menu.EntityID_Item = data.HitEntityID;

            // If item changed then clear options
            if (menu.EntityID_Item != data.HitEntityID)
            {
                menu.ClearOptions();
            }
        }
        
        if (data.HitMenu && data.MouseButton0Up)
        {
            var selectedOption = menu.SelectedOption;
            var optionData = new MenuOptionSelected() { ActorID = data.EntityID, ItemID = menu.EntityID_Item };

            menu.ClearAll();
            
            this.EventManager.QueueEvent(selectedOption, optionData);
        }
    }

    private void SetBusyDots(MenuComponent menu, float dt) 
    {
        menu.BusyDotTimer -= dt;

        if (menu.BusyDotTimer > 0) { return; }

        menu.BusyDotTimer += menu.BusyDotDuration;
        menu.BusyDotsActive++;
        if (menu.BusyDotsActive > 5) { menu.BusyDotsActive = 0; }

        var menuBusy = menu.MenuGO.transform.Find("Menu_Busy").transform as Transform;
        if (menuBusy == null) { return; }

        for (int i = 1; i <= 5; i++)
        {
            menuBusy.Find("Dot"+i).gameObject.SetActive(menu.BusyDotsActive >= i);
        }
    }
    private void SetMenuActive(MenuComponent menu, bool active) 
    {
        if (menu != null)
        {
            menu.MenuGO.SetActive(active);
        }
    }
    private void SetMenuOptions(MenuComponent menu) 
    {
        //menu.RemoveOption(RPGGameEvent.Menu_None);
        //if (menu.MenuOptions.Count == 0 && menu.Distance <= MenuSystem.DistanceItemActionable && menu.EntityID_Item > 0) { menu.AddOption(RPGGameEvent.Menu_None); }

        if (RPGGameEventUtils.MenuOptionIsDisabledType(menu.SelectedOption)) { menu.SelectedOption = RPGGameEvent.None; }

        foreach (Transform child in menu.MenuGO.transform)
        {
            child.gameObject.SetActive(menu.OptionIsActive((RPGGameEvent)Enum.Parse(typeof(RPGGameEvent), child.name, true)));

            float transparency = menu.OptionTransparencyDefault;
            if (child.name != menu.SelectedOption.ToString())
            {
                child.transform.localEulerAngles = new Vector3(child.transform.localEulerAngles.x, menu.OptionAdditionalRotationY, child.transform.localEulerAngles.z);
            }
            else
            {
                if (menu.MouseButtonPressed)
                {
                    child.transform.Rotate(0, child.transform.rotation.y + menu.OptionRotationRate, 0);
                }
                else
                {
                    child.transform.localEulerAngles = new Vector3(child.transform.localEulerAngles.x, menu.OptionAdditionalRotationY, child.transform.localEulerAngles.z);
                }

                transparency = menu.MouseButtonPressed ? menu.OptionTransparencySelected : menu.OptionTransparencyMouseOver;
            }

            var r = menu.MenuGO.transform.FindChild(child.name).GetComponent<Renderer>() as Renderer;
            if (r != null) { r.material.SetColor("_Color", new Color(r.material.color.r, r.material.color.g, r.material.color.b, transparency)); }
        }
    }
    private void SetMenuOptionSpacing(MenuComponent menu) 
    {
        int count = menu.MenuOptions.Count;
        if (count == 0) { return; }

        float menuScaling = Mathf.Clamp(0.3f + (menu.Distance / MenuSystem.DistanceItemActionable - 0.25f) * 0.3f, 0.3f, 1.0f);

        float optionSpacing = MenuOptionSpacing * menuScaling;

        int totalRows = (count > 12) ? 4 : (count > 6) ? 3 : (count > 3) ? 2 : 1;
        int itemsPerRow = Mathf.CeilToInt((float)count / (float)totalRows);

        float startX = -(itemsPerRow - 1) * (optionSpacing / 2);
        float startY = (totalRows - 1) * (optionSpacing / 2);
        float x = startX;
        float y = startY;

        int itemsInRow = 0;
        foreach (var kvp in menu.MenuOptions)
        {
            Transform child = menu.MenuGO.transform.FindChild(kvp.Value.ToString());
            if (child != null)
            {
                child.transform.localPosition = new Vector3(x, y, 0);
                child.transform.localScale = new Vector3(menuScaling, menuScaling, menuScaling);
            }

            itemsInRow++;

            if (itemsInRow >= itemsPerRow)
            {
                itemsInRow = 0;
                y -= optionSpacing;
                x = startX;
            }
            else
            {
                x += optionSpacing;
            }
        }
    }
    private void SetMenuPosition(int entityID, MenuComponent menu) 
    {
        var actorForm = this.EntityManager.GetComponent<FormComponent>(entityID);
        if (actorForm == null) { return; }

        var itemForm = this.EntityManager.GetComponent<FormComponent>(menu.EntityID_Item);
        if (itemForm == null) { return; }

        if (menu.UseCameraForMenuPositioning)
        {
            menu.MenuGO.transform.position = CalcualteMenuPosition(actorForm.CameraPositionCurrent, itemForm.Position_Center);
            menu.MenuGO.transform.rotation = actorForm.CameraRotationNext; 
        }
        else
        {
            menu.MenuGO.transform.position = CalcualteMenuPosition(actorForm.Position, itemForm.Position_Center);
            menu.MenuGO.transform.rotation = actorForm.RotationNext; 
        }
    }

    private static Vector3 CalcualteMenuPosition(Vector3 actorPosition, Vector3 itemPosition) 
    {
        float towardsEntityX = 0.75f;
        float towardsEntityY = 0.60f;
        float towardsEntityZ = 0.75f;

        float x = itemPosition.x * towardsEntityX + actorPosition.x * (1 - towardsEntityX);
        float y = itemPosition.y * towardsEntityY + actorPosition.y * (1 - towardsEntityY);
        float z = itemPosition.z * towardsEntityZ + actorPosition.z * (1 - towardsEntityZ);

        return new Vector3(x, y, z);
    }

    public static void AddOption(int entityID, RPGGameEvent option) 
    {
        var menu = GameBehavior.thisGame.EntityManager.GetComponent<MenuComponent>(entityID);
        if (menu == null) { return; }

        int s = RPGGameEventUtils.MenuSortOrder(option);
        if (!menu.MenuOptions.ContainsKey(s))
        {
            menu.MenuOptions.Add(s, option);
        }
    }
    public static void ClearOptions(int entityID) 
    {
        var menu = GameBehavior.thisGame.EntityManager.GetComponent<MenuComponent>(entityID);
        if (menu == null) { return; }

        menu.ClearOptions();
    }
    public static float DistanceToTarget(int entityID) 
    {
        var menuOptions = GameBehavior.thisGame.EntityManager.GetComponent<MenuComponent>(entityID);
        if (menuOptions == null) { return 10000.0f; }

        return menuOptions.Distance;
    }
    public static bool RemoveOptions(int entityID, List<RPGGameEvent> options) 
    {
        var menu = GameBehavior.thisGame.EntityManager.GetComponent<MenuComponent>(entityID);
        if (menu == null) { return false; }

        foreach (var o in options)
        {
            menu.RemoveOption(o);
        }

        return true;
    }
    public static void SetMenuToBusy(int entityID) 
    {
        var menu = GameBehavior.thisGame.EntityManager.GetComponent<MenuComponent>(entityID);
        if (menu == null) { return; }

        menu.ClearOptions();
        menu.AddOption(RPGGameEvent.Menu_Busy);
        menu.BusyDotsActive = 0;
        menu.BusyDotTimer = menu.BusyDotDuration;
    }
    public static string ToString(int entityID) 
    {
        var menu = GameBehavior.thisGame.EntityManager.GetComponent<MenuComponent>(entityID);

        return (menu == null) ? "" : menu.ToString;
    }

    public class MenuOptionSelected 
    {
        public int ActorID;
        public int ItemID;
    }
}

