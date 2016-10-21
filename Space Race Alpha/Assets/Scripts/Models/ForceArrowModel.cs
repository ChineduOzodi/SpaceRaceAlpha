using UnityEngine;
using System.Collections;
using CodeControl;

public class ForceArrowModel : Model {

    public Vector3d position;
    public Quaternion rotation;
    public Vector3 scale;

    public Color color;

    public ModelRef<PlanetModel> parent;
    public Vector3d force;
}
