using UnityEngine;
using System.Collections;
using CodeControl;

public class ForceArrowModel : Model {

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public Color color;

    public ModelRef<PlanetModel> parent;
    public Vector3 force;
}
