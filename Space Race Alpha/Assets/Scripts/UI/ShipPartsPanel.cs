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

    internal CraftPartModel[] craftModels;

    // Use this for initialization
    void Awake () {

        Model.Load("CraftParts", OnLoadStart, OnLoadProgresss, OnLoadDone, OnLoadError);
        //sprites = Resources.LoadAll("Sprites") as Sprite[];
	
	}

    private void OnLoadProgresss(float obj)
    {
        
    }

    private void OnLoadDone()
    {
        craftModels = Model.GetAll<CraftPartModel>().ToArray();

        foreach (CraftPartModel craftPart in craftModels)
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

        CreateDefualtParts();
    }

    private void CreateDefualtParts()
    {
        CraftPartModel spaceEngine = new CraftPartModel();
        spaceEngine.engines = new EngineComponent[1] { new EngineComponent()};
        spaceEngine.engines[0].mass = Units.Mm;
        spaceEngine.engines[0].specificImpulse = 8.34f * Units.km;
        spaceEngine.engines[0].thrust = 80;
        spaceEngine.engines[0].dimensions = new Vector2(1, 2);
        spaceEngine.name = "Space Engine";
        spaceEngine.spriteName = "space_engine";

        CraftPartModel fuelContainer = new CraftPartModel();
        fuelContainer.containers = new ContainerComponent[1] { new ContainerComponent() };
        fuelContainer.containers[0].type = ContainerTypes.LiquidFuel;
        fuelContainer.containers[0].maxAmount = 2000;
        fuelContainer.containers[0].currentAmount = 2000;
        fuelContainer.containers[0].massPerUnit = 20;
        fuelContainer.containers[0].massEmpty = 200;
        fuelContainer.containers[0].dimensions = new Vector2(3, 10);
        fuelContainer.name = "Fuel Container";
        fuelContainer.spriteName = "ship_fueltank";

        Model.SaveAll("CraftParts");
    }

    private void OnLoadStart()
    {
        
    }

    // Update is called once per frame
    void Update () {
	
	}
}
