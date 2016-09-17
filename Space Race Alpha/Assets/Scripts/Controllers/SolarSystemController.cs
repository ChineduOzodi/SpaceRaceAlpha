using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class SolarSystemController : Controller<SolarSystemModel>
{
    protected override void OnInitialize()
    {
        //Message.AddListener<AddSolarBodyMessage>();
    }

    public void AddSolarBody(Vector3 position, float radius, float mass, ObjectType type = ObjectType.Planet)
    {

    }
}
