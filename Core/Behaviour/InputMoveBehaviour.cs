using Slash.Application.Games;
using Slash.Unity.Common.ECS;
using UnityEngine;


public class InputMoveBehaviour : Slash.Unity.Common.ECS.EntityBehaviour
{
    private void Update() 
    {
        bool crouchToggled = Input.GetKeyUp(KeyCode.LeftControl);
        bool jump = (Input.GetAxis("Jump") > 0);
        bool running = Input.GetKey(KeyCode.LeftShift);

        bool forward = (Input.GetAxis("Vertical") > 0);
        bool backward = (Input.GetAxis("Vertical") < 0);
        bool left = (Input.GetAxis("Horizontal") < 0);
        bool right = (Input.GetAxis("Horizontal") > 0);

        if (forward && backward)
        {
            forward = false;
            backward = false;
        }

        if (left && right)
        {
            left = false;
            right = false;
        }

        this.Game.EventManager.QueueEvent(RPGGameEvent.Input_Move, new MoveData() { EntityID = this.EntityId, Forward = forward, Backward = backward, Left = left, Right = right, CrouchToggled = crouchToggled, Jump = jump, Running = running });
    }

    public class MoveData 
    {
        public int  EntityID;
        public bool Forward;
        public bool Backward;
        public bool Left;
        public bool Right;
        public bool CrouchToggled;
        public bool Jump;
        public bool Running;
    }
}

