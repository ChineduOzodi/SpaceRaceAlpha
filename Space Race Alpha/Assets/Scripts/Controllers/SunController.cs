using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class SunController : Controller<SunModel> {

    public SunModel Model;
    protected override void OnInitialize()
    {
        //setup initial location and rotation
        transform.position = (Vector3) model.position;
        transform.rotation = model.rotation;

        Model = model;
    }

    // Use this for initialization
    void Start () {

        

    }
	
	// Update is called once per frame
	void Update () {

        //model.position = (Vector3d) transform.position;
        //model.rotation = transform.rotation;

    }


    
}
