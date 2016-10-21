using UnityEngine;
using System.Collections;
using CodeControl;

public class TestScript : MonoBehaviour {

    public GameObject sunObject;
    public GameObject planetObject;

    double CM = 0;
    Vector3d CMP = Vector3d.zero;
    Vector3d CMPos = Vector3d.zero;

    double M1 = 0;
    int M1PosInd = 0;

    ModelRefs<SolarBodyModel> objs;

    // Use this for initialization
    void Awake()
    {
        SolarSystemModel sol = new SolarSystemModel();
        SolarSystemController solCont = Controller.Instantiate<SolarSystemController>(sol);
        SolarSystemCreator.AddSun(sol, 5000, 1, "sun");
        //solCont.AddPlanet(new Vector3d(5, 0, 0), 1000, 10);
        //solCont.AddPlanet(new Vector3d(-5, 0, 0), 1000, 10);

        objs = sol.allSolarBodies;
        

        //Figure out "Sun Object" (largest mass)

        for (int b = 0; b < objs.Count; b++)
        {
            if (objs[b].mass > M1)
            {
                M1 = objs[b].mass;
                M1PosInd = b;
            }
        }

        //Figure out "Center of Mass" (largest mass)

        for (int b = 0; b < objs.Count; b++)
        {
            double m1 = objs[b].mass;
            Vector3d r1 = objs[b].position;

            CM += m1;

            CMP += m1 * r1;

        }

        CMPos = CMP / CM;

        //Initial velocities

        for (int i = 0; i < 4; i++)
        {
            for (int b = 0; b < objs.Count; b++)
            {
                double m1 = objs[b].mass;

                Vector3d CMr = CMPos - objs[b].position;
                Vector3d r1 = objs[b].position;

                double M2 = CM - m1;
                Vector3d M2r = CMPos - (CMP - (m1 * r1)) / (M2);

                //Vector3d vel = Forces.AngularVelocity(M2, CMr.magnitude, M2r.magnitude) * Forces.Tangent(CMr.normalized) * CMr.magnitude * .14f;

                Vector3d m2Pos = objs[M1PosInd].position;
                //Vector2 m2Vel = objs[M1PosInd].GetComponent<Rigidbody2D>().velocity;
                Vector3d m2r = m2Pos - objs[b].position;

                for (int c = 0; c < objs.Count; c++)
                {
                    if (c != b)
                    {
                        Vector3d otherDist = objs[c].position - objs[b].position;

                        double m2 = objs[c].mass;

                        if (otherDist.sqrMagnitude < m2r.sqrMagnitude && (otherDist.magnitude - objs[c].SOI) < 0 && m2 > m1)
                        {
                            m2Pos = objs[c].position;
                            Vector3d m2Vel = objs[c].velocity;
                            m2r = m2Pos - objs[b].position;
                            Vector3d CMm2r = CMPos - m2Pos;
                            //CM += m2;
                            //CMP += m2 * m2r;
                            //Vector3d m2force = univGrav(m1, m2, m2r) * Time.deltaTime;

                            //vel = m2Vel / (CMm2r.magnitude) * CMr.magnitude + Forces.AngularVelocity(m2, m2r.magnitude, 0) * Forces.Tangent(m2r.normalized) * m2r.magnitude * .14f;
                        }
                    }


                }


                //Apply Velocity
                //Vector3d force = univGrav(m1, m2, distance) * Time.deltaTime;
                //Vector3d vel = CentripicalForceVel(m1, m2r.magnitude, force.magnitude) * Tangent(force.normalized) + new Vector3d(m2Vel.x, m2Vel.y);
                //objs[b].velocity = vel;


                if (i == 3)
                    Controller.Instantiate<PlanetController>(sunObject, objs[b]);


            }
        }

        sol.showForce = true;
    }

    void Update()
    {
        
    }
}
