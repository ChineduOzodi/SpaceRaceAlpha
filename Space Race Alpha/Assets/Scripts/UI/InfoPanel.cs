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
        if (model.Type == ObjectType.Spacecraft)
        {
            Vector2d apoPeri = Forces.TimeToApoPeri(model);
            double mass = model.mass;

            return string.Format("Mass: {0} kg\nGravity: {1} m/s^2\n Velocity: {6} m/s\nAlt: {2}\nApo: {3}\nPer: {4}\nEcc: {5}\nOrbitalPeriod: {7}\nTime to Apo: {8}\nTime to Peri: {9}",
            mass, 
            (model.force.magnitude / mass).ToString("0.00"), 
            Units.ReadDistance(model.alt),
            Units.ReadDistance(model.PerApo[1] - model.reference.Model.radius),
            Units.ReadDistance(model.PerApo[0] - model.reference.Model.radius),
            model.Ecc.magnitude.ToString("0.00"), 
            model.LocalVelocity.magnitude.ToString("0.00"), //6
            Date.ReadTime(model.OrbitalPeriod),
            Date.ReadTime(apoPeri.x),
            Date.ReadTime(apoPeri.y));
        }
        else
        {
            PlanetModel pmodel = (PlanetModel) model;
            return string.Format("Radius: {1}\nMass: {7}kg\nDensity: {8}\nSurface Gravity: {0} m/s^2\nOrbital Period: {6}\nAlt: {2}\nApo: {3}\nPer: {4}\nRotaion Period: {5}",
            Forces.Force(1,model.mass,pmodel.radius).ToString("0.00"),
            Units.ReadDistance(pmodel.radius),
            Units.ReadDistance(model.alt),
            Units.ReadDistance(model.PerApo[1]),
            Units.ReadDistance(model.PerApo[0]),
            Date.ReadTime(2 * Mathd.PI / model.LocalRotationRate),
            Date.ReadTime(model.OrbitalPeriod), //6
            pmodel.mass,
            pmodel.density); //7

        }
    }
}
