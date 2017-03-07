using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Events;
using UnityEngine;

[GameSystem]
public class InputCameraSystem : GameSystem
{
    private FormComponent _form;
    private Vector2       _mouseAbsolute;
    private Vector2       _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public int     EntityID;
    public Vector2 sensitivity = new Vector2(1, 1);
    public Vector2 smoothing = new Vector2(3, 3);
    public Vector2 targetCharacterDirection;
    public Vector2 targetDirection;

    public override void Init(IAttributeTable configuration) 
    {
        Input.ResetInputAxes();
        this.EventManager.RegisterListener(RPGGameEvent.Input_Mouse, OnMouseInput);
    }

    private void ChangeTargetEntity(int entityID) 
    {
        // Set target direction for the character body to its inital state.
        var form = this.EntityManager.GetComponent<FormComponent>(entityID);
        if (form == null) { return; }

        EntityID = entityID;
        _form = form;

        targetDirection = _form.CameraRotationCurrent.eulerAngles;

        targetCharacterDirection = _form.RotationCurrent.eulerAngles;
    }

    private void OnMouseInput(GameEvent e) 
    {
        var data = (InputMouseBehaviour.MouseData)e.EventData;

        if (data.EntityID != EntityID)
        {
            ChangeTargetEntity(data.EntityID);
        }
        if (_form == null) { return; }

        Cursor.lockState = CursorLockMode.Confined; // Not sure what this state should be

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = new Vector2(data.MouseXRaw, data.MouseYRaw);

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
        {
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
        }

        var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
        _form.CameraRotationNext = xRotation;
        _form.CameraRotationNext *= targetOrientation;

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360)
        {
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
        }

        // If there's a character body that acts as a parent to the camera
        //var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, _form.Rigidbody.transform.up);
        var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, _form.AxisUp);
        _form.RotationNext = yRotation;
        _form.RotationNext *= targetCharacterOrientation;

        _form.CameraRotationNext.eulerAngles = new Vector3(_form.CameraRotationNext.eulerAngles.x, _form.RotationNext.eulerAngles.y, _form.RotationNext.eulerAngles.z);
    }
}

