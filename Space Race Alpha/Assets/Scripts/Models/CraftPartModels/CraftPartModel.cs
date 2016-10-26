using UnityEngine;
using System.Collections;
using CodeControl;

public class CraftPartModel : BaseModel{

    public ModelRef<CraftModel> craft;
    public float cost;

    public EngineComponent[] engines;
    public ContainerComponent[] containers;


    //------------Possible part properties------------------//

    public float dragCo = 1;
    public float TWR
    {
        get
        {
            float totalThrust = 0;
            foreach(EngineComponent engine in engines)
            {
                totalThrust += engine.thrust;
            }
            return totalThrust / (craft.Model.mass * craft.Model.force.magnitude);
        }
    }
    

    
}
