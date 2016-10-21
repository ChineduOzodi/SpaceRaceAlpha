using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class Forces
{

    public static double G = .00000000667; //universal gravity constant

    public static Vector3d Force(BaseModel self, ModelRefs<SolarBodyModel> solarBodies)
    {
        Vector3d force = Vector3d.zero;

        double m1 = self.mass;
        if (solarBodies != null)
        {
            foreach (SolarBodyModel body in solarBodies)
            {
                //TODO: Make more efficient so that they don't have to check force for themselves
                Vector3d m2Pos = body.position;
                double m2 = body.mass;

                Vector3d distance = m2Pos - self.position;
                force += univGrav(m1, m2, distance);

            }
        }


        return force;
    }

    public static Vector3d Force(BaseModel self)
    {
        Vector3d force = Vector3d.zero;

        double m1 = self.mass;
        if (self.reference.Model != null)
        {
            //TODO: Make more efficient so that they don't have to check force for themselves
            Vector3d m2Pos = self.reference.Model.position;
            double m2 = self.reference.Model.mass;

            Vector3d distance = m2Pos - self.position;
            force = univGrav(m1, m2, distance);
        }


        return force;
    }

    public static Vector3d Force(double m1, double m2, Vector3d distance)
    {
        Vector3d force = Vector3d.zero;

        force = univGrav(m1, m2, distance);


        return force;
    }

    public static double Force(double m1, double m2, double distance)
    {

        double force = univGrav(m1, m2, distance);


        return force;
    }
    protected static Vector3d univGrav(double m1, double m2, Vector3d r)
    {
        if (r.magnitude == 0)
            return Vector3d.zero;

        double r2 = Mathd.Pow(r.magnitude, 2);

        Vector3d force = (G * m2 * m1) / r2 * r.normalized;
        //print("Force Added: " + force);
        return force;
    }
    protected static double univGrav(double m1, double m2, double r)
    {
        if (r== 0)
            return 0;

        double r2 = Mathd.Pow(r, 2);

        double force = (G * m2 * m1) / r2;
        //print("Force Added: " + force);
        return force;
    }
    public static Vector3d Tangent(Vector3d normal)
    {
        Vector3d tangent = Vector3d.Cross(normal, Vector3d.forward);

        if (tangent.magnitude == 0)
        {
            tangent = Vector3d.Cross(normal, Vector3d.up);
        }

        return tangent;
    }

    public static double CentripicalForceVel(double m1, double r, double force)
    {
        return Mathd.Sqrt((force * r) / m1);
    }

    public static double AngularVelocity(SolarBodyModel model)
    {
        //return Mathd.Sqrt((G * m2) / (Mathd.Pow(R + r, 2) * r));
        return model.velocity.magnitude / (model.position - model.reference.Model.position).magnitude;
    }
    /// <summary>
    /// Rotate a vector by an angle in radias
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="angle">in radians</param>
    /// <returns></returns>
    internal static Vector3d Rotate(Vector3d vector, double angle)
    {
        if (vector.sqrMagnitude == 0)
        {
            return vector; //to avoid NaN
        }

        return new Polar2(new Polar2(vector).radius, new Polar2(vector).angle + angle).cartesian;
    }

    /// <summary>
    /// Returns the linear velocity tangent to the position vector for a given angular vel
    /// </summary>
    /// <param name="rotationRate">in radians per second</param>
    /// <param name="position">position in meters</param>
    /// <returns></returns>
    public static Vector3d AngularVelocity(double rotationRate, Vector3d position)
    {
        //return Mathd.Sqrt((G * m2) / (Mathd.Pow(R + r, 2) * r));
        return rotationRate * position.magnitude * Tangent(position.normalized);
    }


    internal static Vector3d ForceToVelocity(BaseModel body)
    {
        return (body.force / body.mass) * Time.deltaTime;
    }

    internal static Vector3d ForceToVelocity(Vector3d force, double mass)
    {
        Vector3d acc = force / mass;
        Vector3d vel = acc * Time.deltaTime;
        return vel;
    }

    internal static Vector3d ForceToVelocityRough(BaseModel body, float time)
    {
        return body.force / body.mass * Time.deltaTime * 50 * time;
    }
    /// <summary>
    /// returns it in world position
    /// </summary>
    /// <param name="body">the base model</param>
    /// <returns></returns>
    internal static Vector3d VelocityToPosition(BaseModel body)
    {
        return body.position + body.velocity * Time.deltaTime ;
    }
    internal static Vector3d VelocityToLocalPosition(BaseModel body)
    {
        return body.LocalPosition + body.LocalVelocity * Time.deltaTime;
    }
    internal static Vector3d VelocityToPosition(Vector3d pos, Vector3d vel)
    {
        return pos + vel * Time.deltaTime;
    }

    internal static Vector3 VelocityToPosition(Vector3 pos, Vector3 vel, float time)
    {
        return pos + vel * Time.deltaTime * 50 * time;
    }

    internal static double CircleArea(double radius)
    {
        return Mathd.PI * radius * radius;
    }

    internal static double SphereVolume(double radius)
    {
        return (4.0d * Mathd.PI * radius * radius * radius) / 3.0d;
    }

}