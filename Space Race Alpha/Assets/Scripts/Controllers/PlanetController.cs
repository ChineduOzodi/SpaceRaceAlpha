using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class PlanetController : Controller<PlanetModel> {

    Transform rect;
    internal Rigidbody2D rb2D;

    private void Awake()
    {
        Message.AddListener<AddPlanetMessage>(OnAddPlanetMessage);
        Message.AddListener<ShowForceMessage>(OnShowForceMessage);

        rb2D = GetComponent<Rigidbody2D>();
        rect = GetComponent<RectTransform>();
    }

    private void OnShowForceMessage(ShowForceMessage m)
    {
        Message.RemoveListener<ShowForceMessage>(OnShowForceMessage);

        ForceArrowModel arrow = new ForceArrowModel();
        arrow.color = m.color;
        arrow.parent = new ModelRef<PlanetModel>(model); ;     
        arrow.force = model.force;

        model.showForce = true;

        Controller.Instantiate<ForceController>("forceArrow", arrow);

        

    }

    protected override void OnInitialize()
    {
        //setup initial location and rotation
        rect.position = model.position;
        rect.rotation = model.rotation;
        rect.localScale = model.localScale;

        rb2D.mass = model.mass;
        rb2D.velocity = model.velocity;
    }

    protected override void OnModelChanged()
    {
        //update orgital parameters
        //rect.position = model.position;
        //rect.rotation = model.rotation;
        rect.localScale = model.localScale;


        rb2D.mass = model.mass;
        //rb2D.velocity = model.velocity;
    }

    void Update()
    {
        Vector3 force = Forces.Force(model, model.suns, model.planets);
        model.force = force;
        rb2D.AddForce(force * Time.deltaTime);

        if (force == Vector3.zero)
        {
            model.init = true;
            
            ListUpdate();
        } else

        if(model.init && model.listsUpdated)
        {
            Message.RemoveListener<PlanetAddedMessage>(OnPlanetAddedMessage);
            model.init = false;
            model.listsUpdated = false;
        }
        else if (model.init)
        {
            
            ListUpdate();
        }
        if (model.showForce)
        {
            model.showForce = false;

            ShowForceMessage m = new ShowForceMessage();
            m.color = Color.red;
            m.parent = new ModelRef<PlanetModel>(model);
            m.force = model.force;
            Message.Send(m);
        }

        model.position = rect.position;
        model.rotation = rect.rotation;
        model.localScale = rect.localScale;
        model.mass = rb2D.mass;

        model.NotifyChange();
    }

    private void OnPlanetAddedMessage(PlanetAddedMessage m)
    {
        AddPlanet(m.planet);
        model.listsUpdated = true;
    }

    private void OnAddPlanetMessage(AddPlanetMessage m)
    {
        if (m.planet != model)
        {
            AddPlanet(m.planet);
            PlanetAddedMessage returnM = new PlanetAddedMessage();

            returnM.planet = model;
            Message.Send(returnM);
        }
        

    }

    private void ListUpdate() {
        model.listsUpdated = false;

        AddPlanetMessage m = new AddPlanetMessage();
            m.planet = model;
            Message.Send(m);

            Message.AddListener<PlanetAddedMessage>(OnPlanetAddedMessage);
    }

    private void AddPlanet(PlanetModel p)
    {
        if (p.type == ObjectType.Planet)
            model.planets.Add(p);
    }
}
