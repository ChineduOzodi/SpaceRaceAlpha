using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour {

    private GameObject[] objs;
    internal int simCount = 10;
    public float simScale = 5f;

	// Use this for initialization
	void Start () {

        objs = GameObject.FindGameObjectsWithTag("food");
	}
	
	// Update is called once per frame
	void Update () {

        Vector3[] forces = new Vector3[simCount];
        Vector3[] objsPos = new Vector3[objs.Length];
        Vector3[] objsVel = new Vector3[objs.Length];

        for (int i = 0; i < objs.Length; i++)
        {
            objsPos[i] = objs[i].transform.position;
            objsVel[i] = objs[i].GetComponent<Rigidbody2D>().velocity;
        }

        for (int i = 0; i < simCount; i++)
        {
            for(int b = 0; b < objs.Length; b++)
            {
                float m1 = objs[b].GetComponent<Rigidbody2D>().mass;

                if (objs[b].GetComponent<Trajectory>() != null)
                {
                    objs[b].GetComponent<Trajectory>().vectPos[i] = objsPos[b];
                }

                Vector3 force = Vector3.zero;

                for (int c = 0; c < objs.Length; c++)
                {

                    float m2 = objs[c].GetComponent<Rigidbody2D>().mass;

                    Vector3 distance = objsPos[c] - objsPos[b];

                    force += univGrav(m1, m2, distance);
                }

                forces[i] = force * Time.deltaTime;

                objsVel[b] += (force * Time.fixedDeltaTime * simScale * Time.deltaTime) / m1;
                objsPos[b] += (objsVel[b] * Time.fixedDeltaTime * simScale);

                if (i == 0)
                {
                    objs[b].GetComponent<Rigidbody2D>().AddForce(forces[0]);
                    if (objs[b].GetComponent<Trajectory>() != null)
                    {
                        objs[b].GetComponent<Trajectory>().grav = forces[0];
                    }
                }
            }
        }

        

    }


    public Vector3 univGrav(float m1, float m2, Vector3 distance)
    {
        if (distance == Vector3.zero)
            return Vector3.zero;
        float G = 1f;//.0000000000667f;

        float r3 = Mathf.Pow(distance.sqrMagnitude, 1.5f);

        Vector3 force = (G * m1 * m2 * distance) / r3;
        //print("Force Added: " + force);
        return force;
    }


}
