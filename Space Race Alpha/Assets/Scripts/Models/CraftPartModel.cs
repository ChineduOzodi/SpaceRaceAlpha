using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

public class CraftPartModel: Model {

    public string craftPartName;
    public string spriteName;
    public ModelRef<CraftModel> craft;
    public ModelRef<CraftPartModel> parent;
    public ModelRefs<CraftPartModel> craftParts = new ModelRefs<CraftPartModel>();
    public List<CraftComponents> components;
    public double cost;
    public double dragCo = 1;
    public double partMass;
    public Vector3d dimensions;
    /// <summary>
    /// Position in reference to center of mass
    /// </summary>
    Vector3d cPosition;
    /// <summary>
    /// Position in relation to the parent Craft;
    /// </summary>
    public Vector3d localPosition;
    public double localRotation;
    

    //-----------------Public Fields--------------//

    public double mass
    {
        get
        {
            double m = partMass;
            foreach(CraftComponents component in components)
            {
                m += component.mass;
            }
            return m;
        }
    }


    
    //--------------------Constructors-----------//
    public CraftPartModel() { }

    public CraftPartModel(string _name, string _spriteName, double _mass, List<CraftComponents> _components)
    {

        parent = null;
        craftPartName = _name;
        spriteName = _spriteName;
        partMass = _mass;
        components = _components;
    }

    internal double totalMass()
    {
        double childrenMass = 0;
        foreach (CraftPartModel model in craftParts)
        {
            childrenMass += model.totalMass();
        }

        return mass + childrenMass;
    }

    //--------------------Static Functions------------//
    //-------------Defualt Models------------------//
    public static CraftPartModel BasicCapsule
    {
        get
        {
            List<CraftComponents> components = new List<CraftComponents>();
            components.Add(CapsuleComponent.OnePerson);
            components.Add(ControlComponent.BasicController);
            return new CraftPartModel("Basic Capsule", "ship_capsule", 1000 * Units.km, components);
        }
    }
    public static CraftPartModel LiquidFuelContainer
    {
        get
        {
            List<CraftComponents> components = new List<CraftComponents>();
            components.Add(ContainerComponent.FuelContainer);
            return new CraftPartModel("Liquid Fuel Container", "ship_fueltank", 500 * Units.km, components);
        }
    }

    public static CraftPartModel SpaceEngine
    {
        get
        {
            List<CraftComponents> components = new List<CraftComponents>();
            components.Add(EngineComponent.SpaceEngine);
            return new CraftPartModel("Space Engine", "space_engine", 2000 * Units.km, components);
        }
    }

    //-------------------Functions---------------------//

    /// <summary>
    /// Add a CraftPartModel to this one
    /// </summary>
    /// <param name="model"></param>
    /// <param name="location">location relative to this part</param>
    /// <param name="rotation">local z axis rotation in radians</param>
    public void AddCraftPartModel(CraftPartModel model, Vector3d location, double rotation)
    {

        model.parent = new ModelRef<CraftPartModel>(this);
        model.localPosition = location;
        model.localRotation = rotation;
        model.craft = craft;
        craftParts.Add(model);
    }

    /// <summary>
    /// Calculates the local position of the center of mass in reference to the parent object
    /// </summary>
    /// <returns></returns>
    public Vector3d CalculateMassPosition()
    {
        double coMass = mass;
        Vector3d coPosition = localPosition;
        foreach (CraftPartModel model in craftParts)
        {
            coPosition.x = (coMass * coPosition.x + model.totalMass() * (model.CalculateMassPosition().x + localPosition.x)) / (coMass + model.totalMass());
            coPosition.y = (coMass * coPosition.y + model.totalMass() * (model.CalculateMassPosition().y + localPosition.y)) / (coMass + model.totalMass());
            coMass += model.totalMass();
        }

        return coPosition;
    }
    /// <summary>
    /// Total thrust of all children components of craft
    /// </summary>
    /// <returns></returns>
    public double TotalThrust()
    {
        double totalThrust = 0;
        foreach (CraftComponents comp in components)
        {
            if (comp.componentType == CraftComponentType.Engine)
                totalThrust += ((EngineComponent)comp).thrust;
        }
        foreach (CraftPartModel model in craftParts)
        {
            totalThrust += model.TotalThrust();
        }
        return totalThrust;
    }
    /// <summary>
    /// Vector3 of lowest y position in refernce to the parent object
    /// </summary>
    /// <returns></returns>
    public Vector3d LowestPosition()
    {
        Vector3d lowestP = localPosition;
        lowestP.y -= dimensions.y / 2;
        foreach (CraftPartModel model in craftParts)
        {
            Vector3d lowestP2 = model.LowestPosition() + localPosition;
            lowestP2.y -= dimensions.y / 2;
            if (lowestP2.y < lowestP.y)
            {
                lowestP = lowestP2;
            }
        }

        return lowestP;
    }
}
