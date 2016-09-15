using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class SunController : Controller<SunModel> {

    protected override void OnInitialize()
    {
        //setup initial location and rotation
        GetComponent<RectTransform>().position = model.position;
        GetComponent<RectTransform>().rotation = model.rotation;
        GetComponent<RectTransform>().localScale = model.localScale;
    }

    protected override void OnModelChanged()
    {
        //update orgital parameters
        GetComponent<RectTransform>().position = model.position;
        GetComponent<RectTransform>().rotation = model.rotation;
        GetComponent<RectTransform>().localScale = model.localScale;
    }

    void Update()
    {
        Vector3 force = Forces.Force(model, model.suns, model.planets);
        GetComponent<Rigidbody2D>().AddForce(force * Time.deltaTime);
    }
}
