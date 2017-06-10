using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;

public class ToggleButtonController : Controller<ToggleButtonModel> {

    internal Text label;
    internal Toggle toggle;   

    protected override void OnInitialize()
    {
        label = gameObject.GetComponentInChildren<Text>();
        toggle = gameObject.GetComponentInChildren<Toggle>();

        if (model.addToGroup)
            toggle.group = gameObject.GetComponentInParent<ToggleGroup>();

        transform.localScale = Vector3.one;  //Reset Scale

        toggle.onValueChanged.AddListener((bool bo) => { ValueChanged(bo); });

        label.text = model.label;
        toggle.isOn = false;
    }

    private void ValueChanged(bool bo)
    {
        ToggleMessage m = new ToggleMessage();

        m.label = label.text;
        m.toggleGroupName = model.toggleGroupName;
        m.isToggled = bo;
        m.labelID = model.labeID;

        Message.Send<ToggleMessage>(m);
    }

    protected override void OnModelChanged()
    {
        base.OnModelChanged();
        label.text = model.label;
        Message.Send(model.label);
    }
}
