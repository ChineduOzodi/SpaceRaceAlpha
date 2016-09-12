using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{

	public GameObject gameManager;
	//public bool startClick;
	//public bool exitClick;

	// Use this for initialization
	public void Awake()
	{

		if (GameManager.instance == null)
			Instantiate(gameManager);

	}

	//public void StartClick()
	//{
	//    startClick = true;
	//}
	//public void ExitClick()
	//{
	//    exitClick = true;
	//}

}