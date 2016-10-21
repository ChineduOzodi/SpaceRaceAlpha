using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class ForceController : Controller<ForceArrowModel> {

    Transform rect;
    SpriteRenderer img;
    Vector3d force;

    protected override void OnInitialize()
    {
        rect = transform;
        img = GetComponent<SpriteRenderer>();

        force = model.parent.Model.force;

        rect.position = (Vector3) model.position;
        rect.rotation = model.rotation = Quaternion.identity;
        rect.localScale = model.scale = Vector3.one;

        img.color = model.color;

    }

    //private void OnShowForceMessage(ShowForceMessage m)
    //{
        

    //}

    protected override void OnDestroy()
    {
        //Message.RemoveListener<ShowForceMessage>(OnShowForceMessage);
    }

    protected override void OnModelChanged()
    {
        rect.position = (Vector3) model.position;
        rect.rotation = model.rotation;
        rect.localScale = model.scale;

        img.color = model.color;
    }

    void Update()
    {
        model.force = model.parent.Model.force;

        Polar2 polar = new Polar2(model.force);
        model.rotation = Quaternion.AngleAxis((float) polar.angle * Mathf.Rad2Deg, new Vector3(0,0,1));
        model.scale.x = (float) polar.radius * .001f;

        model.position = model.parent.Model.position;

        model.NotifyChange();

    }
}
