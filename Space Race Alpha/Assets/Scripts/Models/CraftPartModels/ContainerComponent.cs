using UnityEngine;
using System.Collections;
using CodeControl;
/// <summary>
/// Model that stores information for all container types
/// </summary>
public class ContainerComponent : CraftComponents {

    public ContainerTypes containerType;
    public double massEmpty;
    /// <summary>
    /// mass per cubic m in kg
    /// </summary>
    public double massPerUnit;

    /// <summary>
    /// units
    /// </summary>
    public double maxAmount;
    /// <summary>
    /// units
    /// </summary>
    public double currentAmount;

    public new double mass
    {
        get
        {
            return massPerUnit * currentAmount + massEmpty;
        }
    }

    public ContainerComponent()
    {

    }

    public ContainerComponent(ContainerTypes _type, double _massEmpty, double _massPerUnit, double _maxAmount, double _currentAmount)
    {
        containerType = _type;
        massEmpty = _massEmpty;
        massPerUnit = _massPerUnit;
        maxAmount = _maxAmount;
        currentAmount = _currentAmount;
    }
    //-------------Defualt Models------------------//

    public static ContainerComponent FuelContainer
    {
        get
        {
            return new ContainerComponent(ContainerTypes.LiquidFuel, 500, .76f, 2000, 2000);
        }
    }
}
