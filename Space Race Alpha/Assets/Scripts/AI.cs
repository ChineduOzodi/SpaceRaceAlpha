using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : BasePlayer {

	float repeatFreq = .1f;
	bool nofood;
	float enemyMinDist = 10f;
	Vector3 spawnSpot;

	Vector3 closestEnemy;
	Vector3 closestFood;
	Vector3 closestPrey;
	Vector3 closestBorder;

	// Use this for initialization
	void Start () {

		InvokeRepeating ("ScanSurroundings", 0, repeatFreq);
		spawnSpot = transform.position;

	
	}
	
	// Update is called once per frame
	void ScanSurroundings () {
		closestEnemy = Vector3.zero;
		closestFood = Vector3.zero;
		closestPrey = Vector3.zero;
		closestBorder = Vector3.zero;

		Collider2D[] hitColliders = Physics2D.OverlapCircleAll (transform.position, width * 1.5f + 15f);
//		Queue<Collider2D> enemyQ = new Queue<Collider2D> ();
//		Queue<Collider2D> preyQ = new Queue<Collider2D> ();
//		Queue<Collider2D> foodQ = new Queue<Collider2D> ();
		for (int i = 0; i < hitColliders.Length; i++) {
			Collider2D col = hitColliders [i];

			if (col.tag == "food") {
				if (closestFood == Vector3.zero) {
					closestFood = col.transform.position;
				} else if (Vector3.Distance (transform.position, col.transform.position) < Vector3.Distance (transform.position, closestFood)) {
					closestFood = col.transform.position;
				}
			} else if (col.tag == "border") {
				if (closestBorder == Vector3.zero) {
					closestBorder = col.transform.position;
				}else if (Vector3.Distance (transform.position, col.transform.position) < Vector3.Distance (transform.position, closestBorder)) {
					closestBorder = col.transform.position;
				}
			} else {
				BasePlayer colPlayer = col.gameObject.GetComponent<BasePlayer> ();
				if (colPlayer.width > width) {
					if (closestEnemy == Vector3.zero) {
						closestEnemy = col.transform.position;
					}else if (Vector3.Distance (transform.position, col.transform.position) < Vector3.Distance (transform.position, closestEnemy)) {
						closestEnemy = col.transform.position;
					}
				} else if (colPlayer.width < width) {
					if (closestPrey == Vector3.zero) {
						closestPrey = col.transform.position;
					} else if (Vector3.Distance (transform.position, col.transform.position) < Vector3.Distance (transform.position, closestPrey)) {
						closestPrey = col.transform.position;
					}
				}
			}

//			if (col.tag == "food") {
//				foodQ.Enqueue (col);
//			} else if (col.tag == "border") {
//			} else {
//				BasePlayer colPlayer = col.gameObject.GetComponent<BasePlayer> ();
//				if (colPlayer.width > width * (1f + diffScale)) {
//					enemyQ.Enqueue (col);
//				} else if (colPlayer.width < width * (1f - diffScale)) {
//					foodQ.Enqueue (col);
//				}
//			}
		}
	}
	void Update(){
		
		if (Vector3.Distance(closestEnemy,transform.position) < enemyMinDist) {
			offset = closestEnemy - transform.position;
			offset.Scale (new Vector3 (-1f, -1f));
			Move (transform.position + offset);
		} else {
			Move (closestFood);
			//offset = closestFood - transform.position;
			//offset.Scale (new Vector3 (-1f, -1f));
			//offset.Normalize ();
			//offset.Scale (new Vector3 (speedMod / width, speedMod / width));
			//rigid.AddForce(offset);
		}
//		if (enemyQ.Count > 0) {
//			Collider2D col = enemyQ.Dequeue ();
//			Vector3 offset = col.transform.position - transform.position;
//			offset.Scale (new Vector3 (-1f, -1f));
//			Vector3 moveDirection = offset + transform.position;
//			transform.position = Vector3.MoveTowards (transform.position, moveDirection, speedMod / width * Time.deltaTime);
//		} else {
//			int foodSum = 0;
//			Vector3 moveDirection = Vector3.zero;
//			//moveDirection = moveDirection - transform.position;
//			//moveDirection.Normalize ();
//			//print (moveDirection.ToString ());
//			if ((foodQ.Count == 0 || preyQ.Count == 0) && nofood == false) {
//				foodFindpos = GameObject.FindGameObjectWithTag ("food").transform.position;
//				nofood = true;
//			}
//			if (foodQ.Count > 0 || preyQ.Count > 0){
//				nofood = false;
//				while (foodQ.Count > 0) {
//					Collider2D col = foodQ.Dequeue ();
//					float scale = col.transform.GetComponent<Food>().amount / Vector3.Distance (col.transform.position, transform.position);
//					Vector3 offset = col.transform.position - transform.position;
//					offset.Normalize ();
//					offset.Scale (new Vector3 (scale, scale));
//					moveDirection = moveDirection + offset;
//
//				}
//				while (preyQ.Count > 0) {
//					Collider2D col = preyQ.Dequeue ();
//					float scale = col.transform.GetComponent<BasePlayer>().food / Vector3.Distance (col.transform.position, transform.position);
//					Vector3 offset = col.transform.position - transform.position;
//					offset.Normalize ();
//					offset.Scale (new Vector3 (scale, scale));
//					moveDirection = moveDirection + offset;
//
//				}
//
//				moveDirection = moveDirection + transform.position;
//				//print (moveDirection.ToString ());
//				transform.position = Vector3.MoveTowards (transform.position, moveDirection, speedMod / width * Time.deltaTime);
//			} else {
//				transform.position = Vector3.MoveTowards (transform.position, foodFindpos, speedMod / width * Time.deltaTime);
//			}
//		}

	}
}
