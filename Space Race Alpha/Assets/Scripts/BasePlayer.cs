using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BasePlayer : MonoBehaviour {

	public int food = 10;
	public float width = 1.8f;
	protected GameManager gameManager;
	protected float speedMod = 400;
	protected float wallForce = 1;
	protected Text info;
	protected Rigidbody2D rigid;
	protected Vector3 offset;


	protected void Eat(){
		
	}

	public void UpdateSize(){
        if (transform.tag != "food")
        {
            info.text = food.ToString();
        }
        if (food <= 0)
        {
            if (gameObject.tag == "Player")
            {
                if (!gameManager.setup)
                {
                    gameManager.GameOver();
                }
                
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (food > gameManager.highestFoodCount)
        {
            if (gameObject.tag == "Player")
            {
                if (!gameManager.setup)
                {
                    gameManager.GameOver();
                }
            }
            else
            {
                gameManager.highestFoodCount = food;
            }
            
        }
        rigid.mass = food;
        width = Mathf.Sqrt(food / Mathf.PI);
        transform.localScale = new Vector3(width, width);
    }

	void Awake(){
		gameManager = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ();
		info = transform.GetComponentInChildren<Text> ();
		rigid = gameObject.GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate(){

        if (transform.position.x < gameManager.minWorldPosX)
        {
            offset = new Vector3(wallForce, 0);
            rigid.AddForce(offset);
        }
        else if (transform.position.y < gameManager.minWorldPosY)
        {
            offset = new Vector3(0, wallForce);
            rigid.AddForce(offset);
        }
        else if (transform.position.x > gameManager.maxWorldPosX)
        {
            offset = new Vector3(-wallForce, 0);
            rigid.AddForce(offset);
        }
        else if (transform.position.y > gameManager.maxWorldPosY)
        {
            offset = new Vector3(0, -wallForce);
            rigid.AddForce(offset);
        }
    }
	
    protected void Move(Vector3 target) {
        offset = target - transform.position;
        offset.Normalize();
        GameObject obj = Instantiate(gameManager.food, transform.position + offset * width/1.7f, Quaternion.identity) as GameObject;
		obj.GetComponent<Rigidbody2D> ().velocity = rigid.velocity;
        obj.GetComponent<Rigidbody2D> ().AddForce(offset * speedMod);
        rigid.AddForce(offset * -speedMod);
        BasePlayer objPlayer = obj.AddComponent<BasePlayer>();
        food--;
        objPlayer.food = 1;
        UpdateSize();
        objPlayer.UpdateSize();

    }

	void OnTriggerStay2D( Collider2D col){

	
		if (col.gameObject.tag != "border") {
			BasePlayer colScript = col.GetComponent<BasePlayer> ();
			if (colScript.width < width) {
				food ++;
                colScript.food--;
                Rigidbody2D colRigid = col.GetComponent<Rigidbody2D> ();
				Vector2 newVelocity = InelasticCollision (rigid.mass, rigid.velocity, 1f, colRigid.velocity);
                //gameManager.AddFood(1);
				UpdateSize ();
                colScript.UpdateSize();
				rigid.velocity = newVelocity;
            }
            
        }

		
	}

	protected Vector2 InelasticCollision (float mass1, Vector2 vel1, float mass2, Vector2 vel2){

		Vector2 finalVel = (mass1 * vel1 + mass2 * vel2) / (mass1 + mass2);
		return finalVel;
	}

	protected void Attack(){

		GameObject obj = Instantiate (gameManager.bubble,transform.position,Quaternion.identity) as GameObject;
		offset = offset * width;
		obj.GetComponent<Rigidbody2D> ().velocity = rigid.velocity * width * .75f;
		BasePlayer objPlayer = obj.AddComponent<BasePlayer> ();
		obj.tag = transform.tag;
		food = food / 2;
		objPlayer.food = food;
		UpdateSize ();
		objPlayer.UpdateSize ();
	}
}
