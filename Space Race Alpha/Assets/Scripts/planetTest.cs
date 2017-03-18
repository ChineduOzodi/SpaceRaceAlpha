using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class planetTest : MonoBehaviour {

    public Sprite visualSprite;
    // Use this for initialization
    void Start () {

        SolarSystemModel sol = new SolarSystemModel();

        SunModel sun = SolarSystemCreator.AddSun(sol, 6510000, 1.25d, "Sun");

        PlanetModel planet = SolarSystemCreator.AddPlanet(sol, sun, 6400 * Units.km, new Vector3d(.5 * Units.Gm, 0, 0), 5.5, "Earth");
        //planet.Rotation = .25 * Mathd.PI;
        planet.LocalRotationRate = 2 * Mathd.PI/ (24 * Date.Hour);
        PlanetModel moon = SolarSystemCreator.AddPlanet(sol, planet, 1737 * Units.km, new Vector3d(384400 * Units.km, 0, 0), 3.34, "Moon");
        CraftModel craft = SolarSystemCreator.AddCraft(sol, planet, .5 * Mathd.PI, "Craft");
        //CraftModel craft = SolarSystemCreator.AddCraft(sol, planet,new Vector3d( 6500 * Units.km,0,0) , "Craft");
        //Set at target
        Camera.main.GetComponent<CameraController>().targetModel = craft;
        sol.controlModel = new ModelRef<CraftModel>(craft);

        //planet.rotationRate = 360f / (Date.Day);
        SolarSystemController solControl = Controller.Instantiate<SolarSystemController>(sol);
        SpriteRenderer sR = solControl.gameObject.AddComponent<SpriteRenderer>();
        sR.sprite = visualSprite;
        CraftController craftC = Controller.Instantiate<CraftController>(craft.spriteName, craft);

        GetComponent<CameraController>().target = craftC.gameObject;
        GetComponent<CameraController>().targetModel = craft;

        GetComponent<CameraController>().SetTarget(craftC);
        GetComponent<CameraController>().SetControlMode(ControlMode.Craft);

        //Instatiate planet Icon

        //PlanetIconController planetC = Controller.Instantiate<PlanetIconController>("planetIcon", craft.reference.Model);
        //planetC.SetReference(true);
        

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
