using UnityEngine;

public class CraftComponents
{
    public CraftComponentType componentType;
    public Vector2d dimensions;

    /// in kg
    /// </summary>
    public double mass;
    public Vector3d localPosition;

    public double LocalRotation
    {
        get
        {
            return localRotation;
        }
        set
        {
            localRotation = value % (2 * Mathf.PI);
        }
    }

    private double localRotation;

}