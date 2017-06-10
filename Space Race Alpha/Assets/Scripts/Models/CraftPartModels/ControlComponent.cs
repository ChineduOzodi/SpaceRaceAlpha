using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlComponent : CraftComponents {

    public double antennaRange;

	public ControlComponent() { }

    public ControlComponent(double _antennaRange)
    {

        componentType = CraftComponentType.Antenna;
        antennaRange = _antennaRange;
    }

    //-------------Defualt Models------------------//

    public static ControlComponent BasicController
    {
        get
        {
            return new ControlComponent(Units.Gm);
        }
    }
}
