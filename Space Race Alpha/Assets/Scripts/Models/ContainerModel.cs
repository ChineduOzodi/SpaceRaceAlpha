using UnityEngine;
using System.Collections;
using CodeControl;
/// <summary>
/// Model that stores information for all container types
/// </summary>
public class ContainerModel : Model {

    public ContainerTypes type;

    public float massPerUnit;

    public float maxAmount;
    public float currentAmount;
}
