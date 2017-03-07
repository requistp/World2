using System;

public enum RPGGameEvent
{
    // Actions:
    Input_Mouse,
    Input_Mouse_Hit,
    Input_Mouse_Menu_Hit,
    Input_Move,

    // Events:
    ActionStarted,
    AttackHit, // Meaning it did hit the Entity, but there could be adjustments for resistances, etc.
    DamageTaken,
    Death_Broken,
    FallLanded,
    FallStarted,
    JumpLanded,
    JumpStarted,
    TrapDetection,
    TrapDetonated,

    // Menu (in-game)
    Menu_None, // Used when no menu options exist
    Menu_Busy, // Used when a selected action is being performed
    Menu_AttackMelee,
    Menu_Close,//
    Menu_DisarmTrap,//
    Menu_Locked,//
    Menu_Open,//
    Menu_PickLock,//
    Menu_TrapDetected,//
    Menu_UseKey,//

    // Menu Results:
    Closed,
    Opened,
    PickedLock,
    TrapDisarmAttempt,
    UsedKey,

    // System Events:
    AnimationEnded,
    AnimationStarted,

    // Unity Events:
    Unity_OnTrapDetectionRangeEntered,
    Unity_OnAreaOfEffectStay,

    // Special
    None,
}

public static class RPGGameEventUtils
{
    public static bool BusyWithMenuOption(RPGGameEvent action)
    {
        return action.ToString().StartsWith("Menu_");
    }
    public static RPGGameEvent GetEnumFromString(string name) 
    {
        return (RPGGameEvent)Enum.Parse(typeof(RPGGameEvent), name, true);
    }
    public static bool MenuOptionIsDisabledType(RPGGameEvent option) 
    {
        switch (option)
        {
            case RPGGameEvent.Menu_Locked:
            case RPGGameEvent.Menu_None:
            case RPGGameEvent.Menu_TrapDetected:
                return true;

            default:
                return false;
        }
    }
    public static int MenuSortOrder(RPGGameEvent option) 
    {
        switch (option)
        {
            case RPGGameEvent.Menu_None:
                return 0;

            case RPGGameEvent.Menu_Open:
                return 10;
            case RPGGameEvent.Menu_Close:
                return 11;

            case RPGGameEvent.Menu_AttackMelee:
                return 20;

            case RPGGameEvent.Menu_PickLock:
                return 30;
            case RPGGameEvent.Menu_UseKey:
                return 40;
            case RPGGameEvent.Menu_Locked:
                return 50;

            case RPGGameEvent.Menu_DisarmTrap:
                return 60;
            case RPGGameEvent.Menu_TrapDetected:
                return 70;

            default:
                return -1;
        }
    }
}
