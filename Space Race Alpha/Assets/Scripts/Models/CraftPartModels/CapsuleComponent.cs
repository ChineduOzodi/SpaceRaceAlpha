using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleComponent : CraftComponents {

    public double maxCrew;
    public double currentCrew;

	public CapsuleComponent() { }

    public CapsuleComponent(double _maxCrew)
    {

        componentType = CraftComponentType.Command;
        maxCrew = _maxCrew;
    }

    //-------------Defualt Models------------------//

    public static ControlComponent OnePerson
    {
        get
        {
            return new ControlComponent(1);
        }
    }
}
