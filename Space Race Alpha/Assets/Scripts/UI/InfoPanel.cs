using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CodeControl;
using System;

public class InfoPanel : MonoBehaviour {

    internal string targetName;
    internal string info;

    public Text nameText;
    public Text infoText;

    internal BaseModel model;

	void Awake()
    {
        //Add listener
        Message.AddListener<InfoPanelMessage>(OnInfoPanelMessage);

        //set text variables and instantiate text objects

        nameText = Instantiate(nameText, transform) as Text;

        infoText = Instantiate(infoText, transform) as Text;

        
    }

    private void OnInfoPanelMessage(InfoPanelMessage m)
    {
        model = m.model;
    }

    void Update()
    {
        if (model != null)
        {
            nameText.text = model.name;
            infoText.text = GetInfo();
        }
    }

    private string GetInfo()
    {
        if (model.type == ObjectType.Spacecraft)
            return string.Format("Mass: {0} kg\nGravity: {1} m/s^2\n Velocity: {6} m/s\nAlt: {2} km\nApo: {3} km\nPer: {4} km\nEcc: {5}",
            model.mass, (model.force.magnitude / model.mass).ToString("0.00"), (model.alt * .001f).ToString("0.000"), (model.PerApo[1] * .001f).ToString("0.00"), (model.PerApo[0] * .001f).ToString("0.00"), model.Ecc.magnitude.ToString("0.00"), model.velocity.magnitude.ToString("0.00"));
        else
        {
            PlanetModel pmodel = (PlanetModel) model;
            return string.Format("Radius: {1} km\nSurface Gravity: {0} m/s^2\n Orbital Period: {6} yrs\nAlt: {2} km\nApo: {3} km\nPer: {4} km\nRotaion Period: {5} hrs",
            Forces.Force(1,model.mass,pmodel.radius).ToString("0.00"), (pmodel.radius * .001f).ToString("0.00"), (model.alt * .001f).ToString("0.000"), (model.PerApo[1] * .001f).ToString("0.00"), (model.PerApo[0] * .001f).ToString("0.00"), (360 / model.LocalRotationRate / Date.Hour).ToString("0.00"), (model.OrbitalPeriod / Date.Year).ToString("0.00"));

        }
    }
}
