using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelCanvas : MonoBehaviour {

    public GameObject label;
    public Text text;
    internal static LabelCanvas instance;

    bool isEnabled;
    GameObject target;
    private float labelZoomMod = .1f;

    // Use this for initialization
    void Start () {
        if (instance == null)
            instance = this;
	}
	
	// Update is called once per frame
	void Update () {

        if (isEnabled)
        {
            transform.localScale = Vector3.one * Camera.main.orthographicSize * labelZoomMod;
            transform.position = target.transform.position;
        }
    }

    public void SetLabel(GameObject _target, string _text)
    {
        isEnabled = true;
        label.SetActive(true);
        target = _target;
        text.text = _text;
    }

    public void CancelLabel()
    {
        isEnabled = false;
        label.SetActive(false);
    }
}
