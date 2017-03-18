using UnityEngine;
using System.Collections;
using CodeControl;

public class CraftModel : BaseModel
{
    public string craftName;
    public ModelRef<CraftModel> rootCraft;
    /// <summary>
    /// Craft parts attached by either seperation parts or docking parts
    /// </summary>
    public ModelRefs<CraftModel> craftParts = new ModelRefs<CraftModel>();
    /// <summary>
    /// Set whether craft has been spawned into surface view
    /// </summary>
    public bool spawned = false;
    /// <summary>
    /// throttle power from 0 to 100
    /// </summary>
    public float throttle = 0;

    //----------Public Variables---------------//
    public bool isRoot = true;
    public float cost;
    public float dragCo = 1;
    public CraftComponents[] components;

    //----------Private Variables-------------//

    private float Mass;

    //---------Public Fields------------------//

    public new float mass
    {
        get
        {
            return Mass;
        }
    }

    public CraftModel()
    {

    }

    public CraftModel(string _name, string _spriteName, CraftComponents[] _components)
    {
        rootCraft = new ModelRef<CraftModel>(this);
        name = _name;
        spriteName = _spriteName;
        components = _components;
    }


    private float CalculateMass()
    {
        float mass = 0;
        foreach (CraftComponents component in components)
        {
            mass += component.mass;
        }

        return mass;
    }

    //------------Possible part properties------------------//


    public double TWR
    {
        get
        {
            float totalThrust = 0;
            foreach (CraftComponents comp in components)
            {
                if (comp.componentType == CraftComponentType.Engine)
                    totalThrust += ((EngineComponent)comp).thrust;
            }
            return totalThrust / (mass * force.magnitude);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="location"></param>
    /// <param name="rotation">z axis rotation in radians</param>
    public void AddCraftModal(CraftModel model, Vector3 location, double rotation)
    {
        model.rootCraft = rootCraft;
        model.isRoot = false;
        model.reference = new ModelRef<BaseModel>(this);
        model.LocalPosition = (Vector3d)location;
        model.LocalRotation = model.LocalRotation = rotation;
        craftParts.Add(model);
    }

    //-------------Defualt Models------------------//
    public static CraftModel liquidFuelContainer
    {
        get
        {
            return new CraftModel("Liquid Fuel Container", "ship_fueltank", new CraftComponents[] { ContainerComponent.fuelContainer });
        }
    }

    public static CraftModel spaceEngine
    {
        get
        {
            return new CraftModel("Space Engine", "space_engine", new CraftComponents[] { EngineComponent.spaceEngine });
        }
    }

}
