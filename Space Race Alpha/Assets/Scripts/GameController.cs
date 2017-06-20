using CodeControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;
    public SolarSystemModel system;

    public SettingsModel settings;
    public bool setup = true;

    CameraController cam;

    // Use this for initialization
    void Awake () {
        instance = this;
        DontDestroyOnLoad(this);

        system = new SolarSystemModel(9);
        

        //AddCraft
        CraftModel craft = system.allSolarBodies[2].AddCraft(CraftModel.BasicCraft, 0);
        //system.controlModel = new ModelRef<CraftModel>(craft);
        //CraftController craftC = Controller.Instantiate<CraftController>(craft);
        craft = system.allSolarBodies[3].AddCraft(CraftModel.BasicCraft, new Polar2(system.allSolarBodies[3].radius + Units.Mm, 1));

        SolarSystemController solCont = Controller.Instantiate<SolarSystemController>(system);

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        cam.SetCameraView(CameraView.System);

        //cam.SetTarget(craftC);
        //cam.SetViewMode(CameraViewMode.Reference);
        setup = false;
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

                cam.SetTarget(hit.transform.gameObject);

                if (hit.transform.tag == "craftIcon")
                {
                    m.model = hit.transform.GetComponent<CraftIconController>().GetModel();
                }
                else if (hit.transform.tag == "planet")
                {
                    m.model = hit.transform.parent.GetComponent<PlanetController>().Model;
                }
                else if (hit.transform.tag == "planetIcon")
                {
                    m.model = hit.transform.GetComponent<PlanetIconController>().Model;
                }
                else if (hit.transform.tag == "sun")
                {
                    m.model = hit.transform.GetComponent<SunIconController>().Model;
                }

                Message.Send(m);
            }
        }
    }
}
