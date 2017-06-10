using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;

public class AxisNumberController : Controller<AxisNumberModel> {

    internal Text text;

    protected override void OnInitialize()
    {
        text = gameObject.GetComponentInChildren<Text>();
        text.text = model.numberText;

        transform.localScale = Vector3.one;
    }

    protected override void OnModelChanged()
    {
        base.OnModelChanged();

        text.text = model.numberText;
    }

}
