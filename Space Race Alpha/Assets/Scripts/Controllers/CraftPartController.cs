using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using System;

public class CraftPartController : Controller<CraftPartModel> {


    protected override void OnInitialize()
    {
        transform.localPosition = (Vector3)(model.localPosition - model.craft.Model.centerOfMassPosition);

        foreach (CraftPartModel craftPart in model.craftParts)
        {
            Controller.Instantiate<CraftPartController>(craftPart.spriteName, craftPart, this.transform);
        }

    }
	
}
