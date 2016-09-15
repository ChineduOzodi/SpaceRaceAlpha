using UnityEngine;
using System.Collections;
using CodeControl;

public class Forces {


    public static Vector3 Force(PlanetModel self, ModelRefs<SunModel> suns, ModelRefs<PlanetModel> planets)
    {
        Vector3 force = Vector3.zero;

        float m1 = self.mass;

        foreach (SunModel sun in suns)
        {
            Vector3 m2Pos = sun.position;
            float m2 = sun.mass;

            Vector3 distance = m2Pos - self.position;
            force += univGrav(m1, m2, distance) * Time.deltaTime;
        }
        foreach (PlanetModel planet in planets)
        {
            Vector3 m2Pos = planet.position;
            float m2 = planet.mass;

            Vector3 distance = m2Pos - self.position;
            force += univGrav(m1, m2, distance) * Time.deltaTime;
        }

        return force;
    }


    protected static Vector3 univGrav(float m1, float m2, Vector3 r)
    {
        float G = 1; //universal gravity constant

        if (r == Vector3.zero)
            return Vector3.zero;

        float r3 = Mathf.Pow(r.sqrMagnitude, 1.5F);

        Vector3 force = (G * m1 * m2 * r) / r3;
        //print("Force Added: " + force);
        return force;
    }

}

