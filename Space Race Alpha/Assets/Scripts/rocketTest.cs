using UnityEngine;
using System.Collections;
using CodeControl;

public class rocketTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        SolarSystemModel sol = new SolarSystemModel();
        SolarSystemController solCont = Controller.Instantiate<SolarSystemController>(sol);
        solCont.AddPlanet(Vector3.zero, 5000, 100000000);
        var craft = solCont.AddCraft(sol.allSolarBodies[0], 80 * Mathf.Deg2Rad);

        var cam = gameObject.AddComponent<CameraController>();
        cam.SetTarget(craft.gameObject, craft.Model);

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
