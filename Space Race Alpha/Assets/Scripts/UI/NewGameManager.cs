using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameManager : MonoBehaviour {


    public string newGameName;
	// Use this for initialization
	void Start () {
		
	}
	
	public void StartNewGame()
    {
        GameController.instance.system = new SolarSystemModel(9);
    }

    
}
