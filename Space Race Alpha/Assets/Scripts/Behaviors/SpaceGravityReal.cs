using UnityEngine;
using System.Collections;

public class SpaceGravityReal : MonoBehaviour
{
    public float timeScale = 1;
    private GameObject[] objs;
    internal float G = 1000f;
    float CM = 0;
    Vector3 CMP = Vector3.zero;
    Vector3 CMPos = Vector3.zero;

    float M1 = 0;
    int M1PosInd = 0;

    // Use this for initialization
    void Start()
    {
        objs = GameObject.FindGameObjectsWithTag("food");

        //Figure out "Sun Object" (largest mass)

        for (int b = 0; b < objs.Length; b++)
        {
            if (objs[b].GetComponent<Rigidbody2D>().mass > M1)
            {
                M1 = objs[b].GetComponent<Rigidbody2D>().mass;
                M1PosInd = b;
            }
        }

        //Figure out "Center of Mass" (largest mass)

        for (int b = 0; b < objs.Length; b++)
        {
            float m1 = objs[b].GetComponent<Rigidbody2D>().mass;
            Vector3 r1 = objs[b].transform.position;

            CM += m1;

            CMP += m1 * r1;

        }

        CMPos = CMP / CM;

        //Figure out SOI

        for (int b = 0; b < objs.Length; b++)
        {
            float m1 = objs[b].GetComponent<Rigidbody2D>().mass;
            if (objs[b].GetComponent<SpaceTrajectory>() != null)
            {
                objs[b].GetComponent<SpaceTrajectory>().SOI = CalcSOI(m1, CM, Vector3.Distance(CMPos, objs[b].transform.position));
            }

        }

        //Initial velocities

        //for (int i = 0; i < 4; i++)
        //{
        //    for (int b = 0; b < objs.Length; b++)
        //    {
        //        float m1 = objs[b].GetComponent<Rigidbody2D>().mass;

        //        Vector3 CMr = CMPos - objs[b].transform.position;
        //        Vector3 r1 = objs[b].transform.position;

        //        float M2 = CM - m1;
        //        Vector3 M2r = CMPos - (CMP - (m1 * r1)) / (M2);

        //        Vector3 vel = AngularVelocity(M2, CMr.magnitude, M2r.magnitude) * Tangent(CMr.normalized) * CMr.magnitude * .14f;

        //        Vector3 m2Pos = objs[M1PosInd].transform.position;
        //        //Vector2 m2Vel = objs[M1PosInd].GetComponent<Rigidbody2D>().velocity;
        //        Vector3 m2r = m2Pos - objs[b].transform.position;

        //        for (int c = 0; c < objs.Length; c++)
        //        {
        //            if (c != b)
        //            {
        //                Vector3 otherDist = objs[c].transform.position - objs[b].transform.position;

        //                float m2 = objs[c].GetComponent<Rigidbody2D>().mass;

        //                if (otherDist.sqrMagnitude < m2r.sqrMagnitude && (otherDist.magnitude - objs[c].GetComponent<SpaceTrajectory>().SOI) < 0 && m2 > m1)
        //                {
        //                    m2Pos = objs[c].transform.position;
        //                    Vector3 m2Vel = objs[c].GetComponent<Rigidbody2D>().velocity;
        //                    m2r = m2Pos - objs[b].transform.position;
        //                    Vector3 CMm2r = CMPos - m2Pos;
        //                    //CM += m2;
        //                    //CMP += m2 * m2r;
        //                    //Vector3 m2force = univGrav(m1, m2, m2r) * Time.deltaTime;

        //                    vel = m2Vel / (CMm2r.magnitude) * CMr.magnitude + AngularVelocity(m2, m2r.magnitude, 0) * Tangent(m2r.normalized) * m2r.magnitude * .14f;
        //                }
        //            }


        //        }


        //        //Apply Velocity
        //        //Vector3 force = univGrav(m1, m2, distance) * Time.deltaTime;
        //        //Vector3 vel = CentripicalForceVel(m1, m2r.magnitude, force.magnitude) * Tangent(force.normalized) + new Vector3(m2Vel.x, m2Vel.y);
        //        objs[b].GetComponent<Rigidbody2D>().velocity = vel;

        //    }
        //}


    }

    // Update is called once per frame
    void Update()
    {

        //Figure out "Center of Mass" (largest mass)
        CM = 0;
        Vector3 CMP = Vector3.zero;
        Vector3 CMPos = Vector3.zero;

        for (int b = 0; b < objs.Length; b++)
        {
            float m1 = objs[b].GetComponent<Rigidbody2D>().mass;
            Vector3 r1 = objs[b].transform.position;

            CM += m1;

            CMP += m1 * r1;

        }

        CMPos = CMP / CM;

        //Figure out SOI

        for (int b = 0; b < objs.Length; b++)
        {
            float m1 = objs[b].GetComponent<Rigidbody2D>().mass;

            objs[b].GetComponent<SpaceTrajectory>().SOI = CalcSOI(m1, CM, Vector3.Distance(CMPos, objs[b].transform.position));
        }


        //Forces
        for (int b = 0; b < objs.Length; b++)
        {
            float m1 = objs[b].GetComponent<Rigidbody2D>().mass;
            Vector3 CMr = CMPos - objs[b].transform.position;
            //float m2 = 0;
            Vector3 force = Vector3.zero;

            //Setup for Get SOI
            Vector3 SOIm2Pos = objs[M1PosInd].transform.position;
            Vector2 SOIm2Vel = objs[M1PosInd].GetComponent<Rigidbody2D>().velocity;
            Vector3 SOIm2r = SOIm2Pos - objs[b].transform.position;
            float SOIm2 = objs[M1PosInd].GetComponent<Rigidbody2D>().mass;

            for (int c = 0; c < objs.Length; c++)
            {
                if (c != b)
                {
                    Vector3 otherDist = objs[c].transform.position - objs[b].transform.position;

                    Vector3 m2Pos = objs[c].transform.position;
                    float m2 = objs[c].GetComponent<Rigidbody2D>().mass;

                    Vector3 distance = m2Pos - objs[b].transform.position;
                    force += univGrav(m1, m2, distance) * Time.deltaTime;

                    //Get SOI
                    if (otherDist.sqrMagnitude < SOIm2r.sqrMagnitude && (otherDist.magnitude - objs[c].GetComponent<SpaceTrajectory>().SOI) < 0 && m2 > m1)
                    {
                        SOIm2Pos = objs[c].transform.position;
                        SOIm2 = objs[c].GetComponent<Rigidbody2D>().mass;
                        SOIm2Vel = objs[c].GetComponent<Rigidbody2D>().velocity;
                        SOIm2r = SOIm2Pos - objs[b].transform.position;
                    }
                }
            }

            //Apply Force
            //Vector3 force = univGrav(m1, CM, CMr) * Time.deltaTime;
            //Vector3 force = univGrav(m1, m2, distance);
            //Vector3 vel = CentripicalForceVel(m1, distance.magnitude, force.magnitude) * Tangent(force.normalized).normalized;
            //objs[b].GetComponent<Rigidbody2D>().velocity = vel;
            objs[b].GetComponent<Rigidbody2D>().AddForce(force);
            if (objs[b].GetComponent<SpaceTrajectory>() != null)
            {
                objs[b].GetComponent<SpaceTrajectory>().m2Pos = SOIm2Pos;
                objs[b].GetComponent<SpaceTrajectory>().m2Vel = SOIm2Vel;
                objs[b].GetComponent<SpaceTrajectory>().m2 = SOIm2;
                objs[b].GetComponent<SpaceTrajectory>().distance = SOIm2r;
            }


        }
    }

    //void OnDrawGizmos()
    //{
    //    for (int b = 0; b < objs.Length; b++)
    //    {
    //        Gizmos.color = Color.white;
    //        Gizmos.DrawWireSphere(objs[b].transform.position, objs[b].GetComponent<SpaceTrajectory>().SOI);
    //    }



    //}

    public float CalcSOI(float m1, float M1, float r)
    {
        float rSOI = r * Mathf.Pow(m1 / M1, 0.4f);
        return rSOI;
    }

    public Vector3 univGrav(float m1, float m2, Vector3 r)
    {
        if (r == Vector3.zero)
            return Vector3.zero;

        float r3 = Mathf.Pow(r.sqrMagnitude, 1.5F);

        Vector3 force = (G * m1 * m2 * r) / r3;
        //print("Force Added: " + force);
        return force;
    }

    public Vector3 Tangent(Vector3 normal)
    {
        Vector3 tangent = Vector3.Cross(normal, Vector3.forward);

        if (tangent.magnitude == 0)
        {
            tangent = Vector3.Cross(normal, Vector3.up);
        }

        return tangent;
    }

    public float CentripicalForceVel(float m1, float r, float force)
    {
        return Mathf.Sqrt((force * r) / m1);
    }

    public float AngularVelocity(float m2, float r, float R)
    {
        return Mathf.Sqrt((G * m2) / (Mathf.Pow(R + r, 2) * r));
    }
}

