using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using Slash.SystemExt.Utils;
using System;
using UnityEngine;

[GameSystem]
public class InputMovementSystem : GameSystem
{
    private static Vector3 GetDirection(MoveDirection moveDirections) 
    {
        var direction = new Vector3();
        if (moveDirections.IsOptionSet(MoveDirection.Forward))
        {
            direction.z += 1;
        }
        else if (moveDirections.IsOptionSet(MoveDirection.Backward))
        {
            direction.z -= 1;
        }

        if (moveDirections.IsOptionSet(MoveDirection.Right))
        {
            direction.x += 1;
        }
        else if (moveDirections.IsOptionSet(MoveDirection.Left))
        {
            direction.x -= 1;
        }

        return direction;
    }

    public override void Init(IAttributeTable configuration) 
    {
        this.EventManager.RegisterListener(RPGGameEvent.Input_Move, OnMoveInput);
    }

    private void OnMoveInput(GameEvent e) 
    {
        var data = (InputMoveBehaviour.MoveData)e.EventData;

        // Busy?
        if (ActionSystem.IsBusy(data.EntityID)) { return; }

        var movement = this.EntityManager.GetComponent<MovementComponent>(data.EntityID);
        if (movement == null) { return; }

        // Set Movement Direction(s)
        var moveDir = MoveDirection.None;
        if (data.Forward) { moveDir = MoveDirection.Forward; }
        else if (data.Backward) { moveDir = MoveDirection.Backward; }
        if (data.Left) { moveDir = (MoveDirection)MoveDirection.Left.OrOption(moveDir, typeof(MoveDirection)); }
        else if (data.Right) { moveDir = (MoveDirection)MoveDirection.Right.OrOption(moveDir, typeof(MoveDirection)); }

        // Adjust velocity (just direction, not speed)
        movement.Velocity = GetDirection(moveDir); 
    }

    [Flags]
    public enum MoveDirection 
    {
        None,
        Forward = 1 << 1,
        Backward = 1 << 2,
        Left = 1 << 3,
        Right = 1 << 4,
    }
}
