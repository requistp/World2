using Slash.ECS.Components;
using Slash.ECS.Inspector.Attributes;
using Slash.Collections.AttributeTables;
using UnityEngine;

[InspectorComponent]
public class FormComponent : IEntityComponent
{
    public const string AttributePosition = "Transform.Position";
    public const string AttributeRotation = "Transform.Rotation";
    public const string AttributeScale = "Transform.Scale";
    public const string FormPrefabName = "Transform.PrefabName";

    private Camera     _Camera 
    {
        get
        {
            return _UnityGO.transform.FindChild("Camera").GetComponent<Camera>() as Camera;
        }
    }
    private Rigidbody  _Rigidbody 
    {
        get
        {
            return _UnityGO.GetComponent<Rigidbody>() as Rigidbody;
        }
    }
    private GameObject _UnityGO;

    public Vector3    AxisUp 
    {
        get
        {
            return _Rigidbody.transform.up;
        }
    }
    public float      DistanceBelow 
    {
        get
        {
            var dir = new Vector3(0, -1000, 0);
            RaycastHit hit;
            Ray downRay = new Ray(_UnityGO.transform.position, dir);
            if (Physics.Raycast(downRay, out hit))
            {
                return hit.distance - (_UnityGO.transform.localScale.y / 2);
            }
            return 1001;
        }
    }
    public Vector3    CameraPositionCurrent 
    {
        get
        {
            return _Camera.transform.position;
        }
    }
    public Quaternion CameraRotationCurrent 
    {
        get
        {
            return _Camera.transform.localRotation;
        }
    }
    public Quaternion CameraRotationNext;
    public float      NewJumpForce;
    public bool       PhysicsEnabled 
    {
        get
        {
            return (_Rigidbody == null) ? !_Rigidbody.isKinematic : true;
        }
        set
        {
            if (_Rigidbody != null)
            {
                _Rigidbody.isKinematic = !value;
            }
        }
    }
    public string     PrefabPrimary;
    public string     PrefabDeath;
    public Vector3    Position 
    {
        get
        {
            return _UnityGO.transform.position;
        }
    }
    public Vector3    Position_Center 
    {
        get
        {
            var r = _UnityGO.transform.GetComponent<Renderer>() as Renderer;
            return (r != null) ? r.bounds.center : Position;
        }
    }
    public Quaternion RotationCurrent 
    {
        get
        {
            return _UnityGO.transform.rotation;
        }
    }
    public Quaternion RotationNext;
    public Vector3    Velocity 
    {
        get
        {
            return _Rigidbody.velocity;
        }
    }

    public void InitComponent(IAttributeTable attributeTable) 
    {
    }
    public void InitComponent(int entityID, Vector3 position, string prefabName, string deathPrefab) 
    {
        _UnityGO = GameObject.Instantiate(Resources.Load("Prefabs/" + prefabName), position, new Quaternion()) as GameObject;

        var elb = _UnityGO.GetComponent<EntityBehaviour>() as EntityBehaviour;
        if (elb == null) { return; }

        elb.EntityId = entityID;
        elb.Game = GameBehavior.thisGame;

        PrefabPrimary = prefabName;
        PrefabDeath = deathPrefab;
    }

    public GameObject AddChild(string prefabName) 
    {
        var newGO = GameObject.Instantiate(Resources.Load("Prefabs/" + prefabName), _UnityGO.transform.position, _UnityGO.transform.rotation) as GameObject;

        if (newGO != null)
        {
            newGO.name = prefabName;
            newGO.transform.parent = _UnityGO.transform;
        }

        return newGO;
    }
    public Ray        CameraScreenPointToRay(Vector3 position) 
    {
        return _Camera.ScreenPointToRay(position);
    }
    public void       DestroyChild(string name) 
    {
        var c = _UnityGO.transform.FindChild(name) as Transform;
        if (c != null)
        {
            GameObject.Destroy(c.gameObject);
        }
    }
    public void       DestroyForm() 
    {
        GameObject.Destroy(_UnityGO);
    }
    public Animation  PlayAnimation(string name, string sound) 
    {
        SetAudioClip(sound);

        var anim = _UnityGO.GetComponent<Animation>() as Animation;
        if (anim != null)
        {
            anim.Play(name);
            return anim;
        }
        else
        {
            return null;
        }
    }
    public void       SetActive(bool active) 
    {
        _UnityGO.SetActive(active);
    }
    public void       SetAudioClip(string sound) 
    {
        if (sound == "") { return; }

        var audio = _UnityGO.GetComponent<AudioSource>() as AudioSource;
        if (audio != null)
        {
            var ac = GameObject.Instantiate(Resources.Load("Sounds/" + sound)) as AudioClip;
            if (ac != null)
            {
                audio.clip = ac;
            }
        }
    }
    public void       SetCameraRotationToNext() 
    {
        if (CameraRotationNext != null)
        {
            _Camera.transform.rotation = CameraRotationNext;
        }
    }
    public void       SetJumpForce() 
    {
        if (NewJumpForce > 0)
        {
            _Rigidbody.AddForce(0, NewJumpForce, 0, ForceMode.Force);
            NewJumpForce = 0;
        }
    }
    public void       SetRotationToNext() 
    {
        if (RotationNext != null)
        {
            _UnityGO.transform.rotation = RotationNext;
        }
    }
    public void       SetSphereColliderRadius(float radius) 
    {
        var sphereCollider = _UnityGO.GetComponent<SphereCollider>() as SphereCollider;
        if (sphereCollider != null)
        {
            sphereCollider.radius = radius;
        }
    }
    public void       SetSphereColliderRadius(GameObject go, float radius) 
    {
        var sphereCollider = go.GetComponent<SphereCollider>() as SphereCollider;
        if (sphereCollider != null)
        {
            sphereCollider.radius = radius;
        }
    }
    public void       SetSphereColliderRadius(string childName, float radius) 
    {
        if (childName == "")
        {
            SetSphereColliderRadius(_UnityGO, radius);
        }
        else
        {
            var child = _UnityGO.transform.FindChild(childName) as Transform;
            if (child != null)
            {
                SetSphereColliderRadius(child.gameObject, radius);
            }
        }
    }
    public void       TranslatePosition(Vector3 translation) 
    {
        _Rigidbody.transform.Translate(translation);
    }
}

