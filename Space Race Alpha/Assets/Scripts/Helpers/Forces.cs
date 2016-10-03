using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class Forces
{

    public static float G = 1f; //universal gravity constant

    public static Vector3 Force(BaseModel self, ModelRefs<SolarBodyModel> solarBodies)
    {
        Vector3 force = Vector3.zero;

        float m1 = self.mass;
        if (solarBodies != null)
        {
            foreach (SolarBodyModel body in solarBodies)
            {
                //TODO: Make more efficient so that they don't have to check force for themselves
                Vector3 m2Pos = body.position;
                float m2 = body.mass;

                Vector3 distance = m2Pos - self.position;
                force += univGrav(m1, m2, distance);

            }
        }


        return force;
    }

    public static Vector3 Force(BaseModel self)
    {
        Vector3 force = Vector3.zero;

        float m1 = self.mass;
        if (self.reference.Model != null)
        {
            //TODO: Make more efficient so that they don't have to check force for themselves
            Vector3 m2Pos = self.reference.Model.position;
            float m2 = self.reference.Model.mass;

            Vector3 distance = m2Pos - self.position;
            force = univGrav(m1, m2, distance);
        }


        return force;
    }


    //protected static Vector3 univGrav(float m1, float m2, Vector3 r)
    //{
    //    if (r == Vector3.zero)
    //        return Vector3.zero;

    //    float r3 = Mathf.Pow(r.sqrMagnitude, 1.5F);

    //    Vector3 force = (G * m1 * m2 * r) / r3;
    //    //print("Force Added: " + force);
    //    return force;
    //}
    protected static Vector3 univGrav(float m1, float m2, Vector3 r)
    {
        if (r == Vector3.zero)
            return Vector3.zero;

        float r2 = Mathf.Pow(r.magnitude, 2);

        Vector3 force = (G * m2 * m1) / r2 * r.normalized;
        //print("Force Added: " + force);
        return force;
    }
    public static Vector3 Tangent(Vector3 normal)
    {
        Vector3 tangent = Vector3.Cross(normal, Vector3.forward);

        if (tangent.magnitude == 0)
        {
            tangent = Vector3.Cross(normal, Vector3.up);
        }

        return tangent;
    }

    public static float CentripicalForceVel(float m1, float r, float force)
    {
        return Mathf.Sqrt((force * r) / m1);
    }

    public static float AngularVelocity(SolarBodyModel model)
    {
        //return Mathf.Sqrt((G * m2) / (Mathf.Pow(R + r, 2) * r));
        return model.velocity.magnitude / (model.position - model.reference.Model.position).magnitude;
    }
    /// <summary>
    /// Converts Polar (r,0) to cartesian (x,y)
    /// </summary>
    /// <param name="point">(r,0)</param>
    /// <returns>(x,y)</returns>
    public static Vector2 PolarToCartesian(Vector2 point)
    {
        return new Vector2(point.x * Mathf.Cos(point.y), point.x * Mathf.Sin(point.y));
    }
    
    public static Vector2 CartesianToPolar(Vector3 point)
    {
        Vector2 polar;

        float angle = Mathf.Atan(point.y / point.x);

        if (point.x < 0)
        {
            angle += Mathf.PI * 2;
        }
        else if (point.y < 0)
        {
            angle += Mathf.PI;
        }
        else
        {

            angle += Mathf.PI;
        }

        polar = new Vector2(point.magnitude, angle);
        return polar;
    }

    internal static Vector3 ForceToVelocity(BaseModel body)
    {
        return body.force / body.mass * Time.deltaTime;
    }

    internal static Vector3 ForceToVelocity(Vector3 force, float mass)
    {
        return force / mass * Time.deltaTime;
    }

    internal static Vector3 VelocityToPosition(BaseModel body)
    {
        return body.position + body.velocity * Time.deltaTime ;
    }

    internal static float CircleArea(float radius)
    {
        return Mathf.PI * radius * radius;
    }
    

    
}