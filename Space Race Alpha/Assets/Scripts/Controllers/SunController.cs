using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class SunController : Controller<SunModel> {

    protected override void OnInitialize()
    {
        //setup initial location and rotation
        transform.position = model.position;
        transform.rotation = model.rotation;
        transform.localScale = model.localScale;
    }

    protected override void OnModelChanged()
    {
        //update orgital parameters
        transform.position = model.position;
        transform.rotation = model.rotation;
        transform.localScale = model.localScale;
    }
}
