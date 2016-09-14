using UnityEngine;
using System.Collections;
using CodeControl;

public class InitialSolarInitializationManager : MonoBehaviour {

	// Use this for initialization
	void Awake()
    {
        //instantiate sun model
        SunModel sun = new SunModel();
        sun.position = Vector3.zero;
        sun.rotation = Quaternion.identity;
        sun.localScale = Vector3.one;

        //Instantiat sun controller
        Controller.Instantiate<SunController>(sun);

        sun.position.y = 10;
        sun.NotifyChange();
    }
}
