using UnityEngine;
using System.Collections;
using CodeControl;

public class EngineComponent : CraftComponents {

    EngineTypes engineType = EngineTypes.MainEngine;

    /// <summary>
    /// in kilonewtons (kN)
    /// </summary>
    public float thrust;
    /// <summary>
    /// in mass per second
    /// </summary>
    public float specificImpulse;
    /// <summary>


    //Requirements to run engine
    
    /// <summary>
    /// flow rate of engine in Mg/s
    /// </summary>
    float flowRate
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
    public EngineComponent(EngineTypes type, float _mass, float _specificImpulse, float _thrust, Vector3 _localPosition, float _localRotation)
    {
        engineType = type;
        mass = _mass;
        specificImpulse = _specificImpulse;
        thrust = _thrust;
        localPosition = _localPosition;
        LocalRotation = _localRotation;
    }

    //-------------Defualt Models------------------//

    public static EngineComponent spaceEngine
    {
        get
        {
            return new EngineComponent(EngineTypes.MainEngine, Units.Mm, 8.34f * Units.km, 80, Vector3.zero, 180 * Mathf.Deg2Rad);
        }
    }
}
