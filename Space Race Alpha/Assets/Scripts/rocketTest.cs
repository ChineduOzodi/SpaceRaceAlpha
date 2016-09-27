using UnityEngine;
using System.Collections;
using CodeControl;

public class rocketTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        SolarSystemModel sol = new SolarSystemModel();
        SolarSystemController solCont = Controller.Instantiate<SolarSystemController>(sol);
        solCont.AddPlanet(Vector3.zero, 1737000, 7.34f * Mathf.Pow(10,22));

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
