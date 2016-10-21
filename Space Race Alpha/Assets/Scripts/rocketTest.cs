using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class rocketTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        SolarSystemModel sol = new SolarSystemModel();
        SolarSystemController solCont = Controller.Instantiate<SolarSystemController>(sol);
        var sun = SolarSystemCreator.AddSun(sol, Units.Mm, .25, "Sun");

        int numberPlants = UnityEngine.Random.Range(4, 10);
        float minPlantRadius = 10; // in km
        float maxSolarDistance = 100000000; // in GM

        for (int i = 0; i < numberPlants; i++)
        {
            double planetsize = UnityEngine.Random.Range(minPlantRadius, 100); //in km
            Vector3d planetLocation = new Vector3d(UnityEngine.Random.Range(-maxSolarDistance, maxSolarDistance), UnityEngine.Random.Range(-maxSolarDistance, maxSolarDistance), 0); //in gm
            double density = UnityEngine.Random.Range(.1f, 10);

            SolarSystemCreator.AddPlanet(sol, sun, planetsize * Units.hm, planetLocation * Units.Mm, 1, "Planet " + i.ToString());
        }

        //var planet = solCont.AddPlanet(sun.Model, 4f, 15f * new Vector3(-1, 0));
        //var planet2 = solCont.AddPlanet(sun.Model, 50f, 5000000f * new Vector3(1, 0));
        //var moon = solCont.AddPlanet(planet.Model, 2f, 10f * new Vector3(1, 0));
        //solCont.AddPlanet(moon.Model, .5f, 3f * new Vector3(-1, 0));
        //solCont.AddPlanet(new Vector3(0, 10000), 500, 10000000);
        //var craft = solCont.AddCraft(sol.allSolarBodies[1], 90 * Mathf.Deg2Rad);
        var craft = SolarSystemCreator.AddCraft(sol, sol.allSolarBodies[1], Mathd.PI * .5d, "Craft");
        sol.controlModel = new ModelRef<CraftModel>(craft);
        //craft.OnStateChanged(planet2.transform);

        CraftController craftC = Controller.Instantiate<CraftController>("rocket", craft);

        var cam = gameObject.GetComponent<CameraController>();
        cam.SetTarget(craftC);
        cam.SetViewMode(CameraViewMode.Reference);

        

        PlanetController pControl = Controller.Instantiate<PlanetController>("planet", sol.allSolarBodies[1]);
        

        

        //Instatiate planet Icon

        PlanetIconController planet = Controller.Instantiate<PlanetIconController>("planetIcon", craft.reference.Model);
        planet.isReference = true;

        foreach (SolarBodyModel body in craft.reference.Model.solarBodies)
        {
            Controller.Instantiate<PlanetIconController>("planetIcon", body);
        }

        foreach (CraftModel body in craft.reference.Model.crafts)
        {
            Controller.Instantiate<CraftIconController>("craftIcon", body);
        }

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
                    try
                    {
                        m.model = hit.transform.parent.GetComponent<PlanetController>().Model;
                    }
                    catch (Exception b)
                    {
                        m.model = hit.transform.GetComponent<PlanetIconController>().Model;
                    }
                }
                
                Message.Send<InfoPanelMessage>(m);
            }
        }
    }


}
