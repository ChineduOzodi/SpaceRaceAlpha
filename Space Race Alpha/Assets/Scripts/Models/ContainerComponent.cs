using UnityEngine;
using System.Collections;
using CodeControl;
/// <summary>
/// Model that stores information for all container types
/// </summary>
public class ContainerComponent : CraftComponents {

    public ContainerTypes type;
    public float massEmpty;
    /// <summary>
    /// in kg
    /// </summary>
    public float massPerUnit;

    /// <summary>
    /// units
    /// </summary>
    public float maxAmount;
    /// <summary>
    /// units
    /// </summary>
    public float currentAmount;

    public new float mass
    {
        get
        {
            return massPerUnit * currentAmount + massEmpty;
        }
    }
}
