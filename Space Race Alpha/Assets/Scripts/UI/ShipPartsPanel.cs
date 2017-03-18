using UnityEngine;
using System.Collections;
using CodeControl;
using System;
using UnityEngine.UI;

public class ShipPartsPanel : MonoBehaviour {

    public RectTransform content;
    public GameObject craftPartButton;

    public Sprite[] sprites;
    internal Image buttonImage;

    internal CraftModel[] craftModels;

    // Use this for initialization
    void Awake () {

        //Model.Load("CraftParts", OnLoadStart, OnLoadProgresss, OnLoadDone, OnLoadError);
        sprites = Resources.LoadAll("Sprites") as Sprite[];

        craftModels = new CraftModel[] { CraftModel.spaceEngine, CraftModel.liquidFuelContainer };

        foreach (CraftModel craftPart in craftModels)
        {
            GameObject obj = Instantiate(craftPartButton, content) as GameObject;

            buttonImage = obj.GetComponent<Image>();

            foreach (Sprite sprite in sprites)
            {
                if (sprite.name == craftPart.spriteName)
                {
                    buttonImage.sprite = sprite;
                }
            }



            //content.SetS  (content.rect.x, content.rect.y,content.rect.width,content.rect.height + buttonImage.sprite.rect.height);

        }

    }

    private void OnLoadProgresss(float obj)
    {
        
    }

    private void OnLoadDone()
    {
        craftModels = Model.GetAll<CraftModel>().ToArray();

        foreach (CraftModel craftPart in craftModels)
        {
            GameObject obj = Instantiate(craftPartButton, content) as GameObject;

            buttonImage = obj.GetComponent<Image>();

            foreach (Sprite sprite in sprites)
            {
                if (sprite.name == craftPart.spriteName)
                {
                    buttonImage.sprite = sprite;
                }
            }

            

            //content.SetS  (content.rect.x, content.rect.y,content.rect.width,content.rect.height + buttonImage.sprite.rect.height);

        }

        
    }

    private void OnLoadError(string error)
    {
        print("Ship Parts Panel: " + error);
    }

    private void OnLoadStart()
    {
        
    }

    // Update is called once per frame
    void Update () {
	
	}
}
