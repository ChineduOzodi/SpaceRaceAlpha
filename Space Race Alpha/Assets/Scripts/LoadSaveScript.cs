using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;

public class LoadSaveScript: MonoBehaviour {

    public Slider progressSlider;
    public GameObject loadingPanel;

    public Text newGameNameText;


    private SettingsModel settings;

    /// <summary>
    /// Checks to see if SaveModel exists and if not creats and saves it
    /// </summary>
	void Awake()
    {
            Model.Load("Settings", OnLoadStart, OnLoadProgress, OnLoadDone, OnLoadError);
    }

    private void OnLoadError(string err)
    {
        print(err);

        //Create Settings Model

        settings = new SettingsModel();
        settings.Id = "Settings";

        Model.SaveAll("Settings");
    }

    private void OnLoadDone()
    {
        //Close Loading Panel
        loadingPanel.SetActive(false);

        //load settings Model
        settings = Model.First<SettingsModel>();
    }

    private void OnLoadProgress(float progress)
    {
        //Change progress slider
        progressSlider.value = progress;
    }

    private void OnLoadStart()
    {
        //Set loading Panel to active
        loadingPanel.SetActive(true);
    }

    public void OnSaveButtonClicked()
    {

    }
    public void OnNewGameButtonClicked()
    {
        if (newGameNameText.text != "")
        {
            CreateRandomSolarSytem(newGameNameText.text);
        }
    }

    private void CreateRandomSolarSytem(string name)
    {
        SolarSystemModel sol = new SolarSystemModel();
        SolarSystemController solCont = Controller.Instantiate<SolarSystemController>(sol);
        var sun = SolarSystemCreator.AddSun(sol, Units.Mm, .25, "Sun");

        int numberPlants = UnityEngine.Random.Range(4, 10);
        float minPlantRadius = 500; // in km
        float maxSolarDistance = 100000000; // in GM

        for (int i = 0; i < numberPlants; i++)
        {
            double planetsize = UnityEngine.Random.Range(minPlantRadius, (float) sun.radius * .5f + minPlantRadius); //in km
            Vector3d planetLocation = new Vector3d(UnityEngine.Random.Range(-maxSolarDistance, maxSolarDistance), UnityEngine.Random.Range(-maxSolarDistance, maxSolarDistance), 0); //in gm
            double density = UnityEngine.Random.Range(.1f, 10);

            SolarSystemCreator.AddPlanet(sol, sun, planetsize * Units.km, planetLocation * Units.Mm, 1, "Planet " + i.ToString());
        }
    }

    public void OnLoadButtonClicked()
    {

    }
    public void OnExitButtonClicked()
    {

    }
}
