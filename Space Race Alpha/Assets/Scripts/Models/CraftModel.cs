using UnityEngine;
using System.Collections;
using CodeControl;

public class CraftModel : BaseModel {

    /// <summary>
    /// Craft parts attached by either seperation parts or docking parts
    /// </summary>
    public ModelRefs<CraftPartModel> craftParts;
    /// <summary>
    /// Set whether craft has been spawned into surface view
    /// </summary>
    public bool spawned = false;
    /// <summary>
    /// throttle power from 0 to 100
    /// </summary>
    public float throttle = 0;

    //------Flight Info-----//

    


}
