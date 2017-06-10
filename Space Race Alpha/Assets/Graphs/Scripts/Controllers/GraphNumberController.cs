using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;

public class GraphNumberController : Controller<GraphNumberModel> {


    internal Text graphNumText;
    public LayoutElement graphNumTransform;
    internal RectTransform trans;

    float actualHeight;

    protected override void OnInitialize()
    {
        //setup View
        graphNumText = gameObject.GetComponentInChildren<Text>();
        trans = gameObject.GetComponent<RectTransform>();

        CalculateActualHeight();

        graphNumTransform.preferredWidth = model.width;
        graphNumTransform.preferredHeight = 0;
        graphNumText.text = model.label;

        transform.localScale = Vector3.one;

        //AddListeners
        Message.AddListener<ToggleMessage>(OnToggleMessage);

        Invoke("OnModelChanged", 1f);
    }

    private void OnToggleMessage(ToggleMessage obj)
    {
        OnModelChanged();
    }

    protected override void OnModelChanged()
    {
        if (trans != null)
        {
            CalculateActualHeight();

            graphNumTransform.preferredWidth = model.width;

            StopCoroutine("UpdateHeight");
            StartCoroutine("UpdateHeight");
            //graphNumTransform.preferredHeight = actualHeight;
            graphNumText.text = model.label;
        }

        
    }

    protected void CalculateActualHeight()
    {
        float modifier = model.height / model.maxHeight;

        actualHeight = modifier * trans.rect.height;
    }

    IEnumerator UpdateHeight()
    {
        while (graphNumTransform.preferredHeight + 1 < actualHeight || graphNumTransform.preferredHeight - 1 > actualHeight)
        {
            float diff = actualHeight - graphNumTransform.preferredHeight;

            graphNumTransform.preferredHeight += diff / 10;
            yield return null;
        }
        
    }
}
