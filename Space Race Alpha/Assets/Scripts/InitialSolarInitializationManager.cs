using UnityEngine;
using System.Collections;
using CodeControl;

public class InitialSolarInitializationManager : MonoBehaviour {

    public GameObject sunObject;
    public GameObject planetObject;

    float CM = 0;
    Vector3 CMP = Vector3.zero;
    Vector3 CMPos = Vector3.zero;

    // Use this for initialization
    void Awake()
    {
        //instantiate sun model
        SunModel sun = new SunModel();
        sun.position = Vector3.zero;
        sun.rotation = Quaternion.identity;
        sun.localScale = Vector3.one;

        //Instantiat sun controller
        Controller.Instantiate<SunController>(sunObject,sun);

        sun.position.y = 10f;
        sun.localScale.x = 10f;
        sun.NotifyChange();
    }

    void Update()
    {

    }
}
