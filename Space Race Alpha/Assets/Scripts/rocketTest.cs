using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class rocketTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        SolarSystemModel sol = new SolarSystemModel();
        SolarSystemController solCont = Controller.Instantiate<SolarSystemController>(sol);
        var sun = SolarSystemCreator.AddSun(sol, Units.Mm, 1.25, "Sun");

        int numberPlants = UnityEngine.Random.Range(4, 10);
        
        float minPlantRadius = 1000 * Units.km; // in km
        float maxSolarDistance = 150 * Units.Gm; // in GM

        for (int i = 0; i < numberPlants; i++)
        {
            double planetsize = UnityEngine.Random.Range(minPlantRadius, 100 * Units.km); //in km
            Vector3d planetLocation = new Vector3d(UnityEngine.Random.Range(-maxSolarDistance, maxSolarDistance), UnityEngine.Random.Range(-maxSolarDistance, maxSolarDistance), 0); //in gm
            double density = UnityEngine.Random.Range(.1f, 10);

            var planet = SolarSystemCreator.AddPlanet(sol, sun, planetsize, planetLocation, density, "Planet " + i.ToString());

            int numberMoons = UnityEngine.Random.Range(0, 10);

            for (int b = 0; b < numberMoons; b++)
            {
                double moonSize = UnityEngine.Random.Range(10, (float) planet.radius); //in km
                Vector3d moonLocation = new Vector3d(UnityEngine.Random.Range(0, (float)planet.SOI), UnityEngine.Random.Range(0, (float)planet.SOI), 0); //in gm
                density = UnityEngine.Random.Range(.1f, 10);

                SolarSystemCreator.AddPlanet(sol, planet, moonSize, moonLocation, density, "Planet " + i.ToString() + ": Moon " + b.ToString());
            }
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

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.CircleCast(rayPos, .1f, Vector2.up, .1f);

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
                        print(e);
                        m.model = hit.transform.parent.GetComponent<PlanetController>().Model;
                    }
                    catch (Exception b)
                    {
                        print(b);
                        m.model = hit.transform.GetComponent<PlanetIconController>().Model;
                    }
                }
                
                Message.Send(m);
            }
        }
    }


}
