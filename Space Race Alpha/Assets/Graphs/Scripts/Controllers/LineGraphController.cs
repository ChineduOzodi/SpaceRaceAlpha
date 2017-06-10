using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class LineGraphController : Controller<LineGraphModel> {
    //Prefab Search
    
    internal ToggleGroup dataNameGroup;
    internal ToggleGroup dataPreferenceGroup;


    //Public Prefabs
    public Text title;
    public GameObject toggeButton;
    public GameObject graphNumber;
    public GameObject yAxisNumber;
    public GameObject xAxisNumber;

    public RectTransform yAxisParent;
    public RectTransform xAxisParent;
    public RectTransform graphParent;

    //Helper Declarations
    private int yAxisPoints = 0;
    private int xAxisPoints = 0;
    private int maxYValue = 0;
    private List<int> yDataPoints;

    //models
    private ModelRefs<AxisNumberModel> yAxisModels;
    private ModelRefs<AxisNumberModel> xAxisModels;
    private ModelRefs<GraphNumberModel> graphNumberModels;

    protected override void OnInitialize()
    {
        //Find GameObjects
        ToggleGroup[] toggleGroups = gameObject.GetComponentsInChildren<ToggleGroup>();
        dataNameGroup = toggleGroups[0];
        dataPreferenceGroup = toggleGroups[1];

        //Set GameObjects
        yAxisModels = new ModelRefs<AxisNumberModel>();
        xAxisModels = new ModelRefs<AxisNumberModel>();
        graphNumberModels = new ModelRefs<GraphNumberModel>();

        //Reset Scale++
        transform.localScale = Vector3.one;

        //MessageListeners
        Message.AddListener<ToggleMessage>(OnToggleMessage);

        //CreateYAxis
        CalculateMaxYValue();
        

        foreach (int num in yDataPoints)
        {

            //Initiate Models
            AxisNumberModel mod = new AxisNumberModel();
            yAxisModels.Add(mod);

            //Set variables

            mod.numberText = num.ToString();

            //Initialize Controllers
            Controller.Instantiate<AxisNumberController>(yAxisNumber, mod, yAxisParent);



        }

        //Create XAxis
        foreach (float num in model.data[model.selectedDataName,model.selectedDataPreference].Values)
        {
            xAxisPoints++;

            //Initiate Models
            AxisNumberModel mod = new AxisNumberModel();
            GraphNumberModel graphMod = new GraphNumberModel();
            xAxisModels.Add(mod);
            graphNumberModels.Add(graphMod);

            //Set variables
            mod.numberText = "--";

            graphMod.height = num;
            graphMod.maxHeight = maxYValue;
            graphMod.width = 20;
            graphMod.label = num.ToString();

            //Initialize Controllers
            Controller.Instantiate<AxisNumberController>(xAxisNumber, mod, xAxisParent);
            Controller.Instantiate<GraphNumberController>(graphNumber, graphMod, graphParent);



        }

        //Create Toggle Buttons for the Toggle Group Data Names

        CreateToggleGroup("Data Names", model.dataNames, dataNameGroup.transform);
        CreateToggleGroup("Data Prefs", model.dataPrefs, dataPreferenceGroup.transform);

        title.text = model.title;


    }

    private void CalculateMaxYValue()
    {
        float maxNum = 0;
        maxYValue = 0;
        yAxisPoints = 0;
        yDataPoints = new List<int>();
        int increment;

        foreach (float num in model.data[model.selectedDataName, model.selectedDataPreference].Values)
        {
            maxNum = (maxNum < num) ? num : maxNum;
        }

        if (maxNum > 1000)
        {
            increment = 500;
        }
        else if (maxNum > 100)
        {
            increment = 50;
        }
        else if (maxNum > 10)
        {
            increment = 5;
        }
        else
        {
            increment = 1;
        }

        while (maxNum > maxYValue)
        {
            maxYValue += increment;
            yAxisPoints++;
            yDataPoints.Add(maxYValue);
        }

        yDataPoints.Reverse();
    }

    private void OnToggleMessage(ToggleMessage obj)
    {
        if (obj.toggleGroupName == "Data Names")
        {
            model.title = obj.label;
            title.text = model.title;
            model.selectedDataName = obj.labelID;
        }
        else
        {
            model.selectedDataPreference = obj.labelID;
        }
        model.NotifyChange();
    }

    protected override void OnModelChanged()
    {
        //Update YAxis
        CalculateMaxYValue();

        StartCoroutine("UpdateYAxisModels");
        StartCoroutine("UpdateXAxisModels");

        


    }

    IEnumerator UpdateXAxisModels()
    {
        xAxisPoints = model.data[model.selectedDataName, model.selectedDataPreference].Values.Count;

        while ( xAxisModels.Count != xAxisPoints)
        {
            int diff = xAxisModels.Count - xAxisPoints;
            if (diff > 0)
            {
                xAxisModels[0].Delete();
                graphNumberModels[0].Delete();
            }
            else
            {
                //Initiate Models
                AxisNumberModel mod = new AxisNumberModel();
                GraphNumberModel graphMod = new GraphNumberModel();
                xAxisModels.Add(mod);
                graphNumberModels.Add(graphMod);

                //Set variables
                graphMod.width = 20;

                //Initialize Controllers
                Controller.Instantiate<AxisNumberController>(xAxisNumber, mod, xAxisParent);
                Controller.Instantiate<GraphNumberController>(graphNumber, graphMod, graphParent);
            }

            yield return null;
        }
        UpdateXAxisLabels(model.data[model.selectedDataName, model.selectedDataPreference], xAxisModels, graphNumberModels);
    }

    private void UpdateXAxisLabels(Dictionary<string,float> data, ModelRefs<AxisNumberModel> mod, ModelRefs<GraphNumberModel> graphMod)
    {
        int i = 0;
        foreach (KeyValuePair<string,float> dataPair in data)
        {
            mod[i].numberText = dataPair.Key; ;
            mod[i].NotifyChange();

            graphMod[i].maxHeight = maxYValue;
            graphMod[i].height = dataPair.Value;
            graphMod[i].label = dataPair.Value.ToString();
            graphMod[i].NotifyChange();

            i++;
        }
    }

    IEnumerator UpdateYAxisModels()
    {
        while (yAxisModels.Count != yAxisPoints)
        {
            int diff = yAxisModels.Count - yAxisPoints;
            if (diff > 0)
            {
                yAxisModels[0].Delete();
            }
            else
            {
                //Initiate Models
                AxisNumberModel mod = new AxisNumberModel();
                yAxisModels.Add(mod);

                //Initialize Controllers
                Controller.Instantiate<AxisNumberController>(yAxisNumber, mod, yAxisParent);
            }

            yield return null;
        }
        UpdateYAxisLabels(yDataPoints, yAxisModels);
    }

    private void UpdateYAxisLabels(List<int> data, ModelRefs<AxisNumberModel> mod)
    {
        for (int i = 0; i < data.Count; i ++)
        {
            mod[i].numberText = data[i].ToString();

            mod[i].NotifyChange();
        }
    }

    private void CreateToggleGroup(string groupName, string[] dataNames, Transform parent)
    {
        for ( int i = 0; i < dataNames.Length; i++)
        {

            //Init Models
            ToggleButtonModel nameToggle = new ToggleButtonModel();
            nameToggle.labeID = i;
            nameToggle.label = dataNames[i];
            nameToggle.toggleGroupName = groupName;

            //InitController

            ToggleButtonController cont = Controller.Instantiate<ToggleButtonController>(toggeButton, nameToggle, parent);

            //cont.transform.localScale = Vector3.one;
        }
    }
}
