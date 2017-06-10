using UnityEngine;
using System.Collections;
using CodeControl;

public class EngineComponent : CraftComponents {

    EngineTypes engineType = EngineTypes.MainEngine;

    /// <summary>
    /// in kilonewtons (kN)
    /// </summary>
    public double thrust;
    /// <summary>
    /// in mass per second
    /// </summary>
    public double specificImpulse;
    /// <summary>


    //Requirements to run engine
    
    /// <summary>
    /// flow rate of engine in Mg/s
    /// </summary>
    double flowRate
    {
        get
        {
            return thrust / specificImpulse;
        }
    }
    //------------Class Calls----------------------//

    public EngineComponent()
    {

    }
    public EngineComponent(EngineTypes type, double _mass, double _specificImpulse, double _thrust, Vector3d _localPosition, double _localRotation)
    {
        engineType = type;
        componentType = CraftComponentType.Engine;
        mass = _mass;
        specificImpulse = _specificImpulse;
        thrust = _thrust;
        localPosition = _localPosition;
        LocalRotation = _localRotation;
    }

    //-------------Defualt Models------------------//

    public static EngineComponent SpaceEngine
    {
        get
        {
            return new EngineComponent(EngineTypes.MainEngine, Units.Mm, 8.34f * Units.km, 80, Vector3d.zero, 180 * Mathd.Deg2Rad);
        }
    }
}
