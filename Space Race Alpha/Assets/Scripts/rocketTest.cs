using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class rocketTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        SolarSystemModel sol = new SolarSystemModel();
        SolarSystemController solCont = Controller.Instantiate<SolarSystemController>(sol);
        var sun = solCont.AddSun(10000f,"sun",1);
        //var planet = solCont.AddPlanet(sun.Model, 6400000f, 150000000f * new Vector3(1, 0));
        var planet2 = solCont.AddPlanet(sun.Model, 50f, 50000f * new Vector3(1, 0));
        //var moon = solCont.AddPlanet(planet.Model, 2f, 10f * new Vector3(1, 0));
        //solCont.AddPlanet(moon.Model, .5f, 3f * new Vector3(-1, 0));
        //solCont.AddPlanet(new Vector3(0, 10000), 500, 10000000);
        //var craft = solCont.AddCraft(planet2.Model, 90 * Mathf.Deg2Rad);
        var craft = solCont.AddCraft(planet2.Model, 60f * new Vector3(-1, 0));
        //craft.OnStateChanged(planet2.transform);

        var cam = gameObject.AddComponent<CameraController>();
        cam.SetTarget(craft);
        cam.SetViewMode(CameraViewMode.Reference);

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.CircleCast(rayPos, 1, Vector2.up);

            if (hit.collider != null)
            {
                Debug.Log(hit.transform.gameObject.name);

                InfoPanelMessage m = new InfoPanelMessage();
                try{
                    m.model = hit.transform.GetComponent<CraftController>().Model;
                }
                catch (Exception e){
                    m.model = hit.transform.GetComponent<PlanetController>().Model;
                }
                
                Message.Send<InfoPanelMessage>(m);
            }
        }
    }


}
