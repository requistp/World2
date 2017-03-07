using UnityEngine;

public class GameBehavior : Slash.Unity.Common.ECS.GameBehaviour
{
    public static Slash.Application.Games.Game thisGame = null;
    public const float SecondsPerRound = 6.0f;

    public int playerID;

    void Awake()
    {
        base.StartImmediately = false;

        base.Game = new Slash.Application.Games.Game() { GameName = "Phil's Game" };

        thisGame = base.Game;

        AddPlayer();
        AddChest(new Vector3(15, 1, 15), true, true, false);
        AddChest(new Vector3(15, 1, 30), false, false, false);
        AddDoor(new Vector3(15, 0, 45));

        ConfigureGame();

        Game.StartGame();
    }

    void Update()
    {
        Game.Update(Time.deltaTime);
        base.FrameCounter++;
    }

    private void ConfigureGame()
    {
        var moveBehaviour = GetComponent<InputMoveBehaviour>() as InputMoveBehaviour;
        if (moveBehaviour != null)
        {
            moveBehaviour.EntityId = playerID;
            moveBehaviour.Game = Game;
        }

        var mouseBehaviour = GetComponent<InputMouseBehaviour>() as InputMouseBehaviour;
        if (mouseBehaviour != null)
        {
            mouseBehaviour.EntityId = playerID;
            mouseBehaviour.Game = Game;
        }
    }

    private void AddPlayer()
    {
        playerID = Game.EntityManager.CreateEntity();

        var inputBlock = new ActionComponent();
        inputBlock.InitComponent(null);
        Game.EntityManager.AddComponent(playerID, inputBlock);

        var form = new FormComponent();
        form.InitComponent(playerID, new Vector3(0, 50, 0), "Player", "");
        Game.EntityManager.AddComponent(playerID, form);

        var animComp = new AnimationComponent();
        animComp.InitComponent(null);
        Game.EntityManager.AddComponent(playerID, animComp);

        var move = new MovementComponent();
        move.InitComponent(null);
        Game.EntityManager.AddComponent(playerID, move);

        var crouch = new CrouchComponent();
        crouch.InitComponent(null);
        Game.EntityManager.AddComponent(playerID, crouch);

        var run = new RunComponent();
        run.InitComponent(null);
        Game.EntityManager.AddComponent(playerID, run);

        var jump = new JumpComponent();
        jump.InitComponent(null);
        Game.EntityManager.AddComponent(playerID, jump);

        var skill = new SkillComponent();
        skill.InitComponent(null);
        Game.EntityManager.AddComponent(playerID, skill);

        var health = new HealthComponent() { Damage = 0, DeathType = HealthComponent.DeathTypes.LifeEnded, HP_Max = 50 };
        health.InitComponent(null);
        Game.EntityManager.AddComponent(playerID, health);

        var menuOptions = new MenuComponent();
        menuOptions.InitComponent(null);
        menuOptions.InitComponent(playerID, "Menu");
        Game.EntityManager.AddComponent(playerID, menuOptions);
    }
    private void AddChest(Vector3 location, bool locked, bool trapped, bool addHealth)
    {
        int id = Game.EntityManager.CreateEntity();

        var formComp = new FormComponent();
        formComp.InitComponent(id, location, "Chest001", "Chest001_Broken");
        Game.EntityManager.AddComponent(id, formComp);

        var openComp = new OpenComponent();
        openComp.InitComponent(null);
        Game.EntityManager.AddComponent(id, openComp);

        var animComp = new AnimationComponent();
        animComp.InitComponent(null);
        animComp.Animations.Add(new AnimationSystem.AnimationData() { EntityID = id, Name = "ChestClose", Sound = "ChestClose001", DisablePhysics = true, TriggerEvent = RPGGameEvent.Menu_Close });
        animComp.Animations.Add(new AnimationSystem.AnimationData() { EntityID = id, Name = "ChestOpen90", Sound = "ChestOpen001", DisablePhysics = true, TriggerEvent = RPGGameEvent.Menu_Open });
        Game.EntityManager.AddComponent(id, animComp);

        if (locked)
        {
            var lockComp = new LockComponent();
            lockComp.InitComponent(null);
            lockComp.InitComponent(44);
            Game.EntityManager.AddComponent(id, lockComp);
        }

        if (trapped)
        {
            var trapComp = new TrapComponent();
            trapComp.InitComponent(formComp, new EffectSystem.EffectData()
            {
                AreaOfEffectRadius = 10,
                Duration = 5,
                PrefabName = "AOE/Fireball",
                TargetType = EffectSystem.TargetTypeEnum.AreaOfEffect,
                Damage = new DieRollData(2, 4, 0, 0),
                DamageType = AttackSystem.DamageTypes.Crush,
                Description_Attack = "Fireball Blast"
            });
            Game.EntityManager.AddComponent(id, trapComp);
        }

        if (addHealth)
        {
            var health = new HealthComponent() { Damage = 0, DeathType = HealthComponent.DeathTypes.Broken, HP_Max = 5000 };
            Game.EntityManager.AddComponent(id, health);
        }
    }
    private void AddDoor(Vector3 location)
    {
        int id = Game.EntityManager.CreateEntity();

        var formComp = new FormComponent();
        formComp.InitComponent(id, location, "Door001", "");
        Game.EntityManager.AddComponent(id, formComp);

        var openComp = new OpenComponent();
        openComp.InitComponent(null);
        Game.EntityManager.AddComponent(id, openComp);

        var animComp = new AnimationComponent();
        animComp.InitComponent(null);
        animComp.Animations.Add(new AnimationSystem.AnimationData() { EntityID = id, Name = "DoorClose90", Sound = "DoorClose001", DisablePhysics = true, TriggerEvent = RPGGameEvent.Menu_Close });
        animComp.Animations.Add(new AnimationSystem.AnimationData() { EntityID = id, Name = "DoorOpen90", Sound = "DoorOpen001", DisablePhysics = true, TriggerEvent = RPGGameEvent.Menu_Open });
        Game.EntityManager.AddComponent(id, animComp);

        //// Theoretically, this should work if the attribute table is set in the blueprints. Untested
        //var cursorComp = new CursorComponent();
        //cursorComp.InitComponent(null);
        //Game.EntityManager.AddComponent(id, cursorComp);
    }

}
