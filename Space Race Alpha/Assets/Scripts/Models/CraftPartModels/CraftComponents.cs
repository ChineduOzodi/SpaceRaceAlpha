using UnityEngine;

public class CraftComponents
{
    public CraftComponentType componentType;
    public Vector2 dimensions;

    /// in kg
    /// </summary>
    public float mass;
    public Vector3 localPosition;

    public float LocalRotation
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

    private float localRotation;

}