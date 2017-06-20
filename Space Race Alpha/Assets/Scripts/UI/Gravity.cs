using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gravity : MonoBehaviour {
    CameraController cam;
    Image image;
	// Use this for initialization
	void Start () {
        cam = Camera.main.GetComponent<CameraController>();
        image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {

        Vector3d position = (Vector3d) Camera.main.ScreenToWorldPoint(transform.position) * cam.distanceModifier;
        List<Color> colors = new List<Color>();
        float totalForce = 0;
        List<float> forces = new List<float>();

        //Add Reference object
        double distance = position.magnitude;
        float force = (float)Forces.Force(1, cam.reference.mass, distance);
        forces.Add(force);
        colors.Add(cam.reference.color);
        totalForce += force;

        foreach ( SolarBodyModel body in cam.reference.solarBodies)
        {
            distance = (position - body.SystemPosition).magnitude;
            force = (float)Forces.Force(1, body.mass, distance);
            forces.Add(force);
            colors.Add(body.color);
            totalForce += force;
        }
        Color color = new Color(0, 0, 0, .5f);
        for(int i = 0; i < forces.Count; i++)
        {
            float percentForce = (forces[i] / totalForce);
            color.r += colors[i].r * percentForce;
            color.g += colors[i].g * percentForce;
            color.b += colors[i].b * percentForce;
        }
        image.color = color;
		
	}
}
