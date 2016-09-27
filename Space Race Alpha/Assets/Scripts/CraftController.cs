using UnityEngine;
using System.Collections;

public class CraftController : MonoBehaviour {

    internal float throttle = 0;
    internal bool control = true;

    public float translationSpeed = 10f;
    public float rotationSpeed = 10f;
    public float throttleSpeed = 10f;

    internal Rigidbody2D rgb;
    public ParticleSystem prtF;
    public ParticleSystem prtS;

    // Use this for initialization
    void Start () {

        rgb = GetComponent<Rigidbody2D>();
	
	}
	
	// Update is called once per frame
	void Update () {

        if (control)
        {
            float translationV = Input.GetAxis("Vertical") * translationSpeed * Time.deltaTime;
            float translationH = Input.GetAxis("Horizontal") * translationSpeed * Time.deltaTime;
            float rotation = 0;

            if (Input.GetKey(KeyCode.Q))
            {
                rotation = -1 * rotationSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                rotation = 1 * rotationSpeed * Time.deltaTime;
            }

            throttle += (Input.GetKey(KeyCode.LeftShift)) ? 1 * Time.deltaTime : 0;
            throttle -= (Input.GetKey(KeyCode.LeftControl)) ? 1 * Time.deltaTime : 0;

            if (Input.GetKeyDown(KeyCode.X))
            {
                throttle = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                throttle = 100;
            }
            else if ( throttle > 100)
            {
                throttle = 100;
            }
            else if (throttle < 0)
            {
                throttle = 0;
            }
            prtF.startSpeed = throttle * .1f;
            prtS.startSpeed = throttle * .1f;
            throttle = throttle * throttleSpeed;
            


            rgb.AddRelativeForce(new Vector2(translationH, translationV + throttle));
            rgb.AddForce(Vector2.down * 100);
            rgb.AddTorque(rotation);

            //transform.Translate(new Vector3(translationH, translationV, 0));
            //transform.Rotate(new Vector3(0, 0, rotation));


        }
	
	}
}
