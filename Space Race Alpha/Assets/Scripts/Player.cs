using UnityEngine;
using System.Collections;

public class Player : BasePlayer {

	private Vector3 mousePosition;
	private int camSize = 5;
    private int zoomSpeed = 5;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!gameManager.setup) {
			mousePosition = Input.mousePosition;
			mousePosition = Camera.main.ScreenToWorldPoint (mousePosition);
			mousePosition.z = transform.position.z;
			//offset = mousePosition - transform.position;

			//transform.position = Vector3.MoveTowards (transform.position, mousePosition, speedMod/ width * Time.deltaTime);
			//gameObject.GetComponent<Rigidbody2D>().AddForce(offset);
			//Camera.main.orthographicSize = camSize + width * 2;
			Camera.main.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed;
			Camera.main.transform.position = new Vector3(transform.position.x,transform.position.y,-1f);
			//transform.position = Vector2.Lerp (transform.position, mousePosition, speedMod);
			//float transX = Input.GetAxis ("Horizontal") * speedMod * Time.deltaTime;
			//float transY = Input.GetAxis ("Vertical") * speedMod * Time.deltaTime;
			//print (transX.ToString () + transY.ToString ());
			//transform.rotation = new Quaternion();
			//transform.Translate (new Vector3 (transX, transY));
			if(Input.GetButtonDown("Jump")){

                Move(mousePosition);
            }
		}



	}
}
