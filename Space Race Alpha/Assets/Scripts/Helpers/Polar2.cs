using UnityEngine;
using System.Collections;

public struct Polar2 {

    public double radius;
    public double angle;

    public Vector2d cartesian
    {
        get
        {
            return PolarToCartesian(radius, angle);
        }
    }

    /// <summary>
    /// Creates polar coordinates, with angle in radians
    /// </summary>
    /// <param name="_radius">radius</param>
    /// <param name="_angle">angle in radians</param>
	public Polar2(double _radius, double _angle)
    {
        radius = _radius;
        angle = _angle;
    }
    public Polar2( Vector3d point)
    {
        Polar2 polar = CartesianToPolar(point);

        angle = polar.angle;
        radius = polar.radius;
    }
    public Polar2(Vector3 point)
    {
        Polar2 polar = CartesianToPolar(point);

        angle = polar.angle;
        radius = polar.radius;
    }
    //--------------Static Functions----------------//
    /// <summary>
    /// the angle difference from angle1 to angle2 (in radians)
    /// </summary>
    /// <param name="angle1"></param>
    /// <param name="angle2"></param>
    /// <returns></returns>
    public static double Angle(double angle1, double angle2)
    {
        Polar2 angle1Pol = new Polar2(1, angle1);
        Polar2 angle2Pol = new Polar2(1, angle2);

        return Vector2d.Angle(angle1Pol.cartesian, angle2Pol.cartesian) * Mathd.Deg2Rad;
    }
    /// <summary>
    /// Creates x and y variable from polar coords
    /// </summary>
    /// <param name="polar">Polar coords</param>
    /// <returns></returns>
    public static Vector2d PolarToCartesian(Polar2 polar)
    {
        return new Vector2d(polar.radius * Mathd.Cos(polar.angle), polar.radius * Mathd.Sin(polar.angle));
    }

    private static Vector2d PolarToCartesian(double radius, double angle)
    {
        return new Vector2d(radius * Mathd.Cos(angle), radius * Mathd.Sin(angle));
    }
    /// <summary>
    /// Converts Cartesian coords to polar coords with angle in radians
    /// </summary>
    /// <param name="point">Cartesian coordinate</param>
    /// <returns></returns>
    public static Polar2 CartesianToPolar(Vector3d point)
    {
        Polar2 polar;

        double angle = Mathd.Atan(point.y / point.x);

        if (point.x < 0)
        {
            angle += Mathd.PI;
        }
        else if (point.y < 0)
        {
            angle += Mathd.PI * 2;
        }

        polar = new Polar2(point.magnitude, angle);
        return polar;
    }
    /// <summary>
    /// Converts Cartesian coords to polar coords with angle in radians
    /// </summary>
    /// <param name="point">Cartesian coordinate</param>
    /// <returns></returns>
    private static Polar2 CartesianToPolar(Vector3 point)
    {
        Polar2 polar;

        double angle = Mathd.Atan(point.y / point.x);

        if (point.x < 0)
        {
            angle += Mathd.PI;
        }
        else if (point.y < 0)
        {
            angle += Mathd.PI * 2;
        }

        polar = new Polar2(point.magnitude, angle);
        return polar;
    }
}
