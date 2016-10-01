using UnityEngine;
using System.Collections;
using CodeControl;

public class rocketTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        SolarSystemModel sol = new SolarSystemModel();
        SolarSystemController solCont = Controller.Instantiate<SolarSystemController>(sol);
        solCont.AddPlanet(Vector3.zero, 5000, 100000000);
        //solCont.AddPlanet(new Vector3(0, 10000), 500, 10000000);
        var craft = solCont.AddCraft(sol.allSolarBodies[0], 80 * Mathf.Deg2Rad);

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

            if (hit.rigidbody != null)
            {
                Debug.Log(hit.transform.gameObject.name);

                InfoPanelMessage m = new InfoPanelMessage();
                m.model = hit.transform.GetComponent<CraftController>().Model;
                Message.Send<InfoPanelMessage>(m);
            }
        }
    }


}
