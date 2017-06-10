using UnityEngine;
using System.Collections;
using CodeControl;
using System.Collections.Generic;

public class LineGraphTest : MonoBehaviour {

    public GameObject lineGraph;
    public GameObject parentObj;
    internal BarGraphModel model;

    // Use this for initialization
    void Awake() {

        //Init Model
        model = new BarGraphModel();

        model.dataNames = new string[] { "Bushes", "Trees", "Animals" };
        model.dataPrefs = new string[] { "Total", "Rate" };

        model.data = new float[model.dataNames.Length, model.dataPrefs.Length];
        model.data[0, 0] =  300;
        model.data[1, 0] = 460;
        model.data[2, 0] = 234;
        model.data[0, 1] = 10;
        model.data[1, 1] = 6;
        model.data[2, 1] = 23;

        model.selectedDataNames = new List<int>();
        model.selectedDataPreference = 0;



        //Instantiate Controller
        Controller.Instantiate<BarGraphController>(lineGraph, model, parentObj.transform);

        //StartCoroutine("SecondUpdate");
	
	}
	
    //IEnumerator SecondUpdate()
    //{
    //    int count = 0;
    //    for (;;)
    //    {
    //        model.data[0, 0].Add(count++.ToString(), Random.Range(0, 100));
    //        model.data[1, 0].Add(count.ToString(), count + Random.Range(0, 10));
    //        if (model.data[1,0].Count > 10)
    //        {
    //            model.data[1, 0].Remove((count - 10).ToString());
    //        }

    //        model.NotifyChange();

    //        yield return new WaitForSeconds(1);
    //    }
    //}
	// Update is called once per frame
	void Update () {

	
	}
}
