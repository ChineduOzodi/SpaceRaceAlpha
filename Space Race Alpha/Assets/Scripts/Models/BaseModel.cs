using UnityEngine;
using System.Collections;
using CodeControl;

public class BaseModel : Model {

    //name and type info
    public string name;
    public ObjectType type;

    //basic position and orientaion info
    public Vector3 position; //global position in world settings
    public Quaternion rotation = Quaternion.identity; //rotation speed
    public Vector3 localScale = Vector3.one; //scale

    //basic physics info
    public float mass;
    public Vector3 force = Vector3.zero;
    public Vector3 velocity = Vector3.zero;

    //parent object
    public ModelRef<SolarBodyModel> reference;
}
