using UnityEngine;
using System.Collections;
using CodeControl;

public class ShowForceMessage : Message {

    public ModelRef<PlanetModel> parent;
    public Vector3 force;
    public Color color;
}
