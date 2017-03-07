using Slash.Application.Games;
using Slash.Unity.Common.ECS;
using UnityEngine;

public class InputMouseBehaviour : Slash.Unity.Common.ECS.EntityBehaviour
{
    private void Update()
    {
        var form = this.Game.EntityManager.GetComponent<FormComponent>(this.EntityId);
        if (form == null) { return; }

        var data = new MouseData()
        {
            EntityID = this.EntityId,
            Hit = false,
            HitDistance = -1,
            HitEntityID = -1,
            HitMenu = false,
            HitMenuCollider = null,
            HitMenuEntityID = -1,
            MouseButton0 = Input.GetMouseButton(0),
            MouseButton1 = Input.GetMouseButton(1),
            MouseButton2 = Input.GetMouseButton(2),
            MouseButton0Down = Input.GetMouseButtonDown(0),
            MouseButton1Down = Input.GetMouseButtonDown(1),
            MouseButton2Down = Input.GetMouseButtonDown(2),
            MouseButton0Up = Input.GetMouseButtonUp(0),
            MouseButton1Up = Input.GetMouseButtonUp(1),
            MouseButton2Up = Input.GetMouseButtonUp(2),
            MousePosition = Input.mousePosition,
            MouseScrollDelta = Input.mouseScrollDelta,
            MouseScrollWheel = Input.GetAxis("Mouse ScrollWheel"),
            MouseX = Input.GetAxis("Mouse X"),
            MouseY = Input.GetAxis("Mouse Y"),
            MouseXRaw = Input.GetAxisRaw("Mouse X"),
            MouseYRaw = Input.GetAxisRaw("Mouse Y"),
        };

        bool       hit;
        RaycastHit hitRC = new RaycastHit();
        Ray        ray;
        float      rayLength = 100.0f;

        // This puts Ray at center of screen, not the mous as below... ray = form.CameraScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        ray = form.CameraScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(form.CameraPositionCurrent, ray.direction * rayLength, Color.gray, 0.1f);
        hit = Physics.Raycast(ray, out hitRC, rayLength, LayerMask.GetMask("MouseTargetable"));
        if (hit)
        {
            data.Hit = true;
            data.HitDistance = hitRC.distance;
            //Debug.DrawLine(ray.origin, hitRC.point, Color.blue, 0.1f);
            var elb = hitRC.transform.root.GetComponent<EntityBehaviour>() as EntityBehaviour;
            if (elb != null)
            {
                data.HitEntityID = elb.EntityId;
            } //else { Debug.Log("elb is null"); }
        }

        hit = Physics.Raycast(ray, out hitRC, rayLength, LayerMask.GetMask("Menu"));
        if (hit)
        {
            data.HitMenu = true;
            Debug.DrawLine(ray.origin, hitRC.point, Color.green, 0.1f);
            var elb = hitRC.transform.root.GetComponent<EntityBehaviour>() as EntityBehaviour;
            if (elb != null)
            {
                data.HitMenuEntityID = elb.EntityId;
                data.HitMenuCollider = hitRC.collider;
                // Debug.Log(hitRC.collider.name);
            } //else { Debug.Log("elb is null"); }
        }

        this.Game.EventManager.QueueEvent(RPGGameEvent.Input_Mouse, data);
        if (data.Hit) { this.Game.EventManager.QueueEvent(RPGGameEvent.Input_Mouse_Hit, data); }
        if (data.HitMenu) { this.Game.EventManager.QueueEvent(RPGGameEvent.Input_Mouse_Menu_Hit, data); }
    }

    public class MouseData
    {
        public int      EntityID;
        public bool     Hit;
        public float    HitDistance;
        public int      HitEntityID;
        public bool     HitMenu;
        public Collider HitMenuCollider;
        public int      HitMenuEntityID;
        public bool     MouseButton0;
        public bool     MouseButton1;
        public bool     MouseButton2;
        public bool     MouseButton0Down;
        public bool     MouseButton1Down;
        public bool     MouseButton2Down;
        public bool     MouseButton0Up;
        public bool     MouseButton1Up;
        public bool     MouseButton2Up;
        public Vector3  MousePosition;
        public Vector2  MouseScrollDelta;
        public float    MouseScrollWheel;
        public float    MouseX;
        public float    MouseXRaw;
        public float    MouseY;
        public float    MouseYRaw;
    }
}

