using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class planetTest : MonoBehaviour {

    public Sprite visualSprite;
    // Use this for initialization
    void Start () {

        SolarSystemModel sol = new SolarSystemModel();

        SunModel sun = SolarSystemCreator.AddSun(sol, 6510000, .35d, "Sun");

        PlanetModel planet = SolarSystemCreator.AddPlanet(sol, sun, 640 * Units.km, new Vector3d(Units.Gm, 0, 0), 5.5, "Earth");
        planet.RotationRate = 0;
        //PlanetModel moon = SolarSystemCreator.AddPlanet(sol, planet, 5000, new Vector3d(150 * Units.km, 0, 0), 1, "Moon");
        CraftModel craft = SolarSystemCreator.AddCraft(sol, planet, .5 * Mathd.PI, "Craft");

        //Set at target
        Camera.main.GetComponent<CameraController>().targetModel = craft;
        sol.controlModel = new ModelRef<CraftModel>(craft);

        //planet.rotationRate = 360f / (Date.Day);
        SolarSystemController solControl = Controller.Instantiate<SolarSystemController>(sol);
        SpriteRenderer sR = solControl.gameObject.AddComponent<SpriteRenderer>();
        sR.sprite = visualSprite;
        CraftController craftC = Controller.Instantiate<CraftController>("rocket", craft);

        GetComponent<CameraController>().target = craftC.gameObject;
        GetComponent<CameraController>().targetModel = craft;

        GetComponent<CameraController>().SetTarget(craftC);
        GetComponent<CameraController>().SetControlMode(ControlMode.Craft);

        //Instatiate planet Icon

        PlanetIconController planetC = Controller.Instantiate<PlanetIconController>("planetIcon", craft.reference.Model);
        planetC.SetReference(true);

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
                try
                {
                    m.model = hit.transform.GetComponent<CraftController>().Model;
                }
                catch (Exception e)
                {
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
