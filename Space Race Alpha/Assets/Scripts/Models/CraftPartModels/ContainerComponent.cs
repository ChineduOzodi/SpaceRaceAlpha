using UnityEngine;
using System.Collections;
using CodeControl;
/// <summary>
/// Model that stores information for all container types
/// </summary>
public class ContainerComponent : CraftComponents {

    public ContainerTypes containerType;
    public float massEmpty;
    /// <summary>
    /// mass per cubic m in kg
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

    public ContainerComponent()
    {

    }

    public ContainerComponent(ContainerTypes _type, float _massEmpty, float _massPerUnit, float _maxAmount, float _currentAmount)
    {
        containerType = _type;
        massEmpty = _massEmpty;
        massPerUnit = _massPerUnit;
        maxAmount = _maxAmount;
        currentAmount = _currentAmount;
    }
    //-------------Defualt Models------------------//

    public static ContainerComponent fuelContainer
    {
        get
        {
            return new ContainerComponent(ContainerTypes.LiquidFuel, 500, .76f, 2000, 2000);
        }
    }
}
