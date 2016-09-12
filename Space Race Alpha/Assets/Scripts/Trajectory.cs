using UnityEngine;
using System.Collections;

public class Trajectory : MonoBehaviour
{
    public Color c1 = Color.red;
    public Color c2 = Color.yellow;
    internal int verts = 1000; //Can't be greater than simcount in gravity code

    internal Vector3[] vectPos;

    internal Vector2 grav = Vector2.zero;
    // Use this for initialization
    void Start()
    {
        vectPos = new Vector3[verts];
    }

    // Update is called once per frame
    void Update()
    {

        DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity);

    }

    public void DrawTraject(Vector2 startPos, Vector2 startVelocity)
    {
        var line = gameObject.GetComponent<LineRenderer>();
        line.SetVertexCount(verts);
        line.SetWidth(.2f, .2f);
        line.SetColors(c1, c2);

        line.SetPositions(vectPos);
    }
}
