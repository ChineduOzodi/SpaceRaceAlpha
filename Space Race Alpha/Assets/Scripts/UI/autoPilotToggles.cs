using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class autoPilotToggles : MonoBehaviour {

    public Toggle[] toggles;

    internal CraftController target; //target object
    internal BaseModel targetModel; //targetModel

    internal GameObject reference;
    internal ToggleGroup toggleGroup;

    CameraController camContr;

    // Use this for initialization
    void Start()
    {

        camContr = Camera.main.GetComponent<CameraController>();
        toggleGroup = GetComponent<ToggleGroup>();
    }

    // Update is called once per frame
    void Update()
    {

        if (target == null)
        {
            if (camContr.target != null)
            {
                target = camContr.target.GetComponent<CraftController>();
            }
            
        }
        else
        {
            //Autopilot buttons
            if (Input.GetKeyDown(KeyCode.T))
            {
                target.ToggleSAS();
                if (target.SAS)
                {
                    toggles[0].isOn = true;
                    toggleGroup.NotifyToggleOn(toggles[0]);
                }

                else
                {
                    toggles[0].isOn = false;
                    toggleGroup.SetAllTogglesOff();
                }
                    
            }
        }

    }

    public void ToggleSAS()
    {
        target.ToggleSAS();
    }
    public void TogglePro()
    {
        target.TogglePrograde();
    }
    public void ToggleRetro()
    {
        target.ToggleRetrograde();
    }
}
