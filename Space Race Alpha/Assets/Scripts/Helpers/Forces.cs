using UnityEngine;
using System.Collections;
using CodeControl;
using System;

public class Forces
{

    public static double G = .0000000000667; //universal gravity constant
    /// <summary>
    /// Calculates accumulative force from solar system acting on object.
    /// Also changes the bodies reference if needed so another foreach call doesn't have to be made
    /// </summary>
    /// <param name="self"></param>
    /// <param name="solarBodies"></param>
    /// <returns></returns>
    public static Vector3d Force(BaseModel self, ModelRefs<SolarBodyModel> solarBodies)
    {
        Vector3d totalForce = Vector3d.zero;
        double m1 = self.mass;

        Vector3d referenceDistance = self.reference.Model.SystemPosition - self.SystemPosition;
        Vector3d referenceForce = univGrav(m1, self.reference.Model.mass, referenceDistance);

        
        if (solarBodies != null)
        {
            foreach (SolarBodyModel body in solarBodies)
            {
                if (body.name != self.name)
                {
                    Vector3d m2Pos = body.SystemPosition;
                    double m2 = body.mass;
                    Vector3d distance = m2Pos - self.SystemPosition;

                    Vector3d force = univGrav(m1, m2, distance);
                    totalForce += force;

                    CheckReference(self, force, referenceForce, referenceDistance, body, distance);
                }




            }
        }


        return totalForce;
    }

    private static void CheckReference(BaseModel self, Vector3d newForce, Vector3d referenceForce, Vector3d referenceDistance, SolarBodyModel potentialBody, Vector3d potentialBodyDistance)
    {
        //-------------Changes Reference if needed-----------------//
        if (newForce.sqrMagnitude > referenceForce.sqrMagnitude && potentialBody.mass > self.mass
            && (((SolarBodyModel)self.reference.Model).SOI < referenceDistance.magnitude ||
            potentialBody.SOI > potentialBodyDistance.magnitude))
        {
            if (self.Type != ObjectType.Spacecraft)
            {
                ((SolarBodyModel)self.reference.Model).solarBodies.Remove((SolarBodyModel)self);
                potentialBody.solarBodies.Add((SolarBodyModel)self);

                self.reference = new ModelRef<SolarBodyModel>(potentialBody);

                ((SolarBodyModel)self).CalculateSOI();
            }
        }
    }

    public static Vector3d Force(BaseModel self, bool considerReferenceBodies)
    {
        Vector3d totalForce = Vector3d.zero;
        double m1 = self.mass;

        Vector3d referenceDistance = self.reference.Model.SystemPosition - self.SystemPosition;
        Vector3d referenceForce = univGrav(m1, self.reference.Model.mass, referenceDistance);

        if (considerReferenceBodies)
        {
            if (self.reference.Model.reference.Model.name != self.reference.Model.name)
            {
                Vector3d m2Pos = self.reference.Model.reference.Model.SystemPosition;
                Vector3d distance = m2Pos - self.SystemPosition;
                double m2 = self.reference.Model.reference.Model.mass;
                Vector3d force = univGrav(m1, m2, distance);
                totalForce += force;

                CheckReference(self, force, referenceForce, referenceDistance, (SolarBodyModel) self.reference.Model.reference.Model, distance);
            }
            

            foreach (SolarBodyModel body in ((SolarBodyModel)self.reference.Model).solarBodies)
            {
                


                if (body.name != self.name)
                {
                    Vector3d m2Pos = body.SystemPosition;
                    double m2 = body.mass;
                    Vector3d distance = m2Pos - self.SystemPosition;

                    Vector3d force = univGrav(m1, m2, distance);
                    totalForce += force;

                    CheckReference(self, force, referenceForce, referenceDistance, body, distance);

                }
            }

            
        }

        if (self.reference.Model != null)
        {
            //TODO: Make more efficient so that they don't have to check force for themselves
            Vector3d m2Pos = self.reference.Model.SystemPosition;
            double m2 = self.reference.Model.mass;

            Vector3d distance = m2Pos - self.SystemPosition;

            Vector3d force = univGrav(m1, m2, distance);
            totalForce += force;
        }

        return totalForce;
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
        return model.velocity.magnitude / (model.SystemPosition - model.reference.Model.SystemPosition).magnitude;
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


    internal static Vector3d ForceToVelocity(BaseModel body, double deltaTime)
    {
        return (body.force / body.mass) * deltaTime;
    }

    internal static Vector3d ForceToVelocity(BaseModel body, Vector3d addedForce, double deltaTime)
    {
        return ((body.force + addedForce) / body.mass) * deltaTime;
    }
    internal static Vector3d ForceToVelocity(Vector3d force, double mass, double deltaTime)
    {
        Vector3d acc = force / mass;
        Vector3d vel = acc * deltaTime;
        return vel;
    }
    /// <summary>
    /// Gives a rough estimate of velocity by applying static force over given amount of time
    /// </summary>
    /// <param name="body"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    internal static Vector3d ForceToVelocityRough(BaseModel body, double time)
    {
        return body.force / body.mass * time;
    }
    /// <summary>
    /// Gives a rough estimate of velocity by applying static force over given amount of time
    /// </summary>
    /// <param name="force"></param>
    /// <param name="mass"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    internal static Vector3d ForceToVelocityRough(Vector3d force, double mass, double time)
    {
        return force / mass * time;
    }
    /// <summary>
    /// change in position
    /// </summary>
    /// <param name="body">the base model</param>
    /// <returns></returns>
    internal static Vector3d VelocityToPosition(BaseModel body, double deltaTime)
    {
        return body.velocity * deltaTime ;
    }
    /// <summary>
    /// Returns the position after a velocity has been applied for a certain amount of time
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="vel"></param>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    internal static Vector3d VelocityToPosition(Vector3d pos, Vector3d vel, double deltaTime)
    {
        return pos + vel * deltaTime;
    }

    internal static double CircleArea(double radius)
    {
        return Mathd.PI * radius * radius;
    }

    internal static double SphereVolume(double radius)
    {
        return (4.0d/3.0d) * Mathd.PI * Mathd.Pow(radius,3);
    }

    internal static Vector2d TimeToApoPeri (BaseModel model)
    {
        double previousVerticalVelocity = model.SurfaceVel.y;
        double currentVerticalVelocity = model.SurfaceVel.y;
        double timeToApo = 0;
        double timeToPeri = 0;

        double time = 0;
        int counter = 0;
        Vector3d locPos = model.LocalPosition;
        Vector3d locVel = model.LocalVelocity;
        Vector3d force = model.force;

        while (timeToApo == 0 || timeToPeri == 0)
        {
            double timeMulti = 1;
            time += timeMulti;
            currentVerticalVelocity = Forces.Rotate(locVel, -new Polar2(locPos).angle + .5 * Mathd.PI).y;
            
            locPos = VelocityToPosition(locPos, locVel, timeMulti);
            force = -Force(model.mass, model.reference.Model.mass, locPos);
            locVel += ForceToVelocityRough(force, model.mass, timeMulti);
            
            

            if ( previousVerticalVelocity < 0 && currentVerticalVelocity > 0)
            {
                timeToPeri = time;
            }
            else if (previousVerticalVelocity > 0 && currentVerticalVelocity < 0)
            {
                timeToApo = time;
            }

            previousVerticalVelocity = currentVerticalVelocity;
            counter++;
            if (counter > 10000)
            {
                break;
            }
        }

        return new Vector2d(timeToApo, timeToPeri);
    }
}