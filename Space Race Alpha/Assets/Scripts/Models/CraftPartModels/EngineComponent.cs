using UnityEngine;
using System.Collections;
using CodeControl;

public class EngineComponent : CraftComponents {

    EngineTypes type = EngineTypes.MainEngine;
    Direction direction = Direction.Down;

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
}
