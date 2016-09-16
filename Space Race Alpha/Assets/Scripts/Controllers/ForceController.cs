using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class ForceController : Controller<ForceArrowModel> {

    Transform rect;
    SpriteRenderer img;
    Vector3 force;

    protected override void OnInitialize()
    {
        rect = transform;
        img = GetComponent<SpriteRenderer>();

        force = model.parent.Model.force;

        rect.position = model.position;
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
        rect.position = model.position;
        rect.rotation = model.rotation;
        rect.localScale = model.scale;

        img.color = model.color;
    }

    void Update()
    {
        model.force = model.parent.Model.force;

        Vector2 polar = Forces.CartesianToPolar(model.force * -1);
        model.rotation = Quaternion.AngleAxis(polar.y * Mathf.Rad2Deg, new Vector3(0,0,1));
        model.scale.x = polar.x * .001f;

        model.position = model.parent.Model.position;

        model.NotifyChange();

    }
}
