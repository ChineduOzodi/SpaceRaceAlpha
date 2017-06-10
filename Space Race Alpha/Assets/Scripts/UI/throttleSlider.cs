using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class throttleSlider : MonoBehaviour {

    public CraftController target; //target object
    public BaseModel targetModel; //targetModel

    public GameObject reference;
    public Slider slider;

    CameraController camContr;

    // Use this for initialization
    void Start()
    {

        camContr = Camera.main.GetComponent<CameraController>();
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {

        if (target == null)
        {
            if (camContr.target != null)
                target = camContr.target.GetComponent<CraftController>();
        }
        else
        {


            slider.value = target.throttle;
        }

    }
}
