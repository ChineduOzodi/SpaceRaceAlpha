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
        SolarSystemModel sol = new SolarSystemModel(9);
        SolarSystemController solCont = Controller.Instantiate<SolarSystemController>(sol);
    }

    public void OnLoadButtonClicked()
    {

    }
    public void OnExitButtonClicked()
    {

    }
}
