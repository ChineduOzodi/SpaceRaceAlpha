using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour
{
    #region "Declarations"
    //Game Settings
    public Vector2 gridWorldSize;
    public float nodeRadius = 1;
    public LayerMask unwalkableMask;

    public int foodLimit;
	public int aiCount;
    private int minFoodSize = 1;
    private int maxFoodSize = 200;

    public Shader playerShader;

    [HideInInspector]
    public int highestFoodCount = 0;

    public float minWorldPosX;
    public float minWorldPosY;
    public float maxWorldPosX;
    public float maxWorldPosY;

    // Save and Setup
    public static GameManager instance = null;
    List<Scorer> highScores;
    private float lowestHighScoreTime;
    string playerName;
    string savePathPlayerName;
    string savePathHighScores;
    public bool setup = true;
    public bool menu = true;

    //Game Setup
	public GameObject border;
	public GameObject food;
	public GameObject bubble;
    private GameObject player;
    
	GameObject borderEmpty;
	GameObject foodEmpty;
    Grid grid;

    float timer;

    //Main Menu UI Setup
    private Button menuStartButton;
    private Button menuExitButton;
    private Text highScoreText;

    //Game UI
    public Text gameInfo;
    private Text gameTimer;
    
    private GameObject gameOverPanel;
    private Text gameOverText;
    private Button gameExitButton;
    private Button gameRestartButton;
    private InputField menuEnterNameField;
    #endregion
    #region "Monodevelop functions"
    // Use this for initialization
    void Awake()
	{
        //Initial Setup and making sure only one gamemanager exists
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

        //Load saved info
		savePathHighScores = Application.persistentDataPath + "highscores.sav";
        savePathPlayerName = Application.persistentDataPath + "playername.sav";
        Load();
        RunMainMenuSetup();
        
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            ExitGame();
        }
        if (!menu)
        {
            if (!setup)
            {
                Timer();
                gameInfo.text = "Food Amount to Win: " + highestFoodCount;
            }

        }
    }
    void OnLevelWasLoaded(int levelInt)
    {
        if(levelInt == 0) //Main Menu
        {
            menu = true;
            RunMainMenuSetup();
            
        }
        else if (levelInt == 1) //Game Level
        {
            menu = false;
            RunGameSetup();
            timer = 0;
        }
    }

    #endregion
    private void RunMainMenuSetup()
    {
        //Button Setup
        setup = true;
        menuStartButton = GameObject.FindGameObjectWithTag("start_button").GetComponent<Button>();
        menuExitButton = GameObject.FindGameObjectWithTag("exit_button").GetComponent<Button>();
        menuEnterNameField = GameObject.FindGameObjectWithTag("name_input_field").GetComponent<InputField>();
        highScoreText = GameObject.FindGameObjectWithTag("info").GetComponent<Text>();

        menuStartButton.onClick.AddListener(() => { StartGame(); });
        menuExitButton.onClick.AddListener(() => { ExitGame(); });
        menuEnterNameField.onEndEdit.AddListener(delegate { SaveName(); });

        //Name Setup
        menuEnterNameField.text = playerName;

        //HighScores Setup
        HighScores();
        

        setup = false;
    }

    private void HighScores()
    {
        string highScoresString = "";

        for (int i = 0; i < highScores.Count; i++)
        {
            int m = i + 1;
            string playerString = "";
            if (m.ToString().Length == 2)
            {
                playerString = String.Format("{0}. {1} s - {2}\n", m.ToString(), highScores[i].time.ToString(), highScores[i].name);
            }
            else
            {
                playerString = String.Format("{0}.  {1} s - {2}\n", m.ToString(), highScores[i].time.ToString(), highScores[i].name);
            }
            
            if (playerString.Length < 40)
            {
                string dashes = new string('-', 40 - playerString.Length);
                playerString = playerString.Insert(playerString.IndexOf('-'), dashes);
            }
            
            highScoresString += playerString;

        }
        highScoreText.text = highScoresString;
    }

    internal void ExitGame()
    {
        if (menu)
        {
            Application.Quit();
        }
        else
        {
            setup = true;
            SceneManager.LoadScene("main_menu");
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene("level");
    }

    internal void GameOver()
    {
        setup = true;
        gameOverPanel.SetActive(true);
        if (player.GetComponent<Player>().food > highestFoodCount && timer < lowestHighScoreTime)
        {
            // New High Score
            gameOverText.text = "You made it on the Leaderboard!!\n" + timer.ToString() + " s";
            for (int i = 0; i < highScores.Count; i++)
            {
                if (timer < highScores[i].time)
                {
                    highScores.Insert(i, new Scorer(playerName, Mathf.RoundToInt(timer)));
                    break;
                }
            }
            highScores.RemoveAt(10);
            SaveHighScores();
        }
        else if (player.GetComponent<Player>().food > highestFoodCount)
        {
            //Won Game, no new high score
            gameOverText.text = "You Won!!\n" + timer.ToString() + " s";
        }
        else
        {
            //Lost Game
            gameOverText.text = "Game Over";
        }
    }

    private void SaveName()
    {
        playerName = menuEnterNameField.text;

        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(savePathPlayerName);

        bf.Serialize(file, playerName);
        file.Close();
    }

    public void SaveHighScores()
	{
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(savePathHighScores);

        bf.Serialize(file, highScores);
        file.Close();

        
    }
	public void Load()
	{
        if (File.Exists(savePathHighScores))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePathHighScores, FileMode.Open);
            highScores = (List<Scorer>)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            highScores = new List<Scorer>()
            {
                new Scorer(), new Scorer(), new Scorer(), new Scorer(), new Scorer(),
                new Scorer(), new Scorer(), new Scorer(), new Scorer(), new Scorer()}; 
        }
        if (File.Exists(savePathPlayerName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePathPlayerName, FileMode.Open);
            playerName = (string)bf.Deserialize(file);
            file.Close();
        }
        else { playerName = "Player Name"; }

        lowestHighScoreTime = highScores[9].time;
    }

	private void RunGameSetup(){
		setup = true;
        highestFoodCount = 0;
        //Setup Game UI
        gameRestartButton = GameObject.FindGameObjectWithTag("restart_button").GetComponent<Button>();
        gameExitButton = GameObject.FindGameObjectWithTag("exit_button").GetComponent<Button>();

        gameInfo = GameObject.FindGameObjectWithTag("info").GetComponent<Text>();
        gameTimer = GameObject.FindGameObjectWithTag("timer").GetComponent<Text>();

        gameOverPanel = GameObject.FindGameObjectWithTag("game_over_panel");
        gameOverText = GameObject.FindGameObjectWithTag("game_over_text").GetComponent<Text>();

        gameRestartButton.onClick.AddListener(() => { StartGame(); });
        gameExitButton.onClick.AddListener(() => { ExitGame(); });

        //Hide Game Over Panel
        gameOverPanel.SetActive(false);

        //Create Collision check grid
        grid = new GameObject("Grid").AddComponent<Grid>();

        setup = true;

        borderEmpty = new GameObject("border");
        foodEmpty = new GameObject("food");

        minWorldPosX = grid.grid[0, 0].worldPosition.x;
        minWorldPosY = grid.grid[0, 0].worldPosition.y;
        maxWorldPosX = grid.grid[grid.gridSizeX-1, grid.gridSizeY-1].worldPosition.x;
        maxWorldPosY = grid.grid[grid.gridSizeX-1, grid.gridSizeY-1].worldPosition.y;

        for (int x = 0; x < grid.gridSizeX; x++) {
			for (int y = 0; y < grid.gridSizeY; y++) {
				if (x == 0 || y == 0 || x + 1 == grid.gridSizeX || y + 1 == grid.gridSizeY) {
					GameObject borderBlock = Instantiate (border, grid.grid[x, y].worldPosition, Quaternion.identity) as GameObject;
					borderBlock.transform.SetParent (borderEmpty.transform);
				}
			}
					
		}

        grid.CreateGrid();

        // spawn Food

        for (int i = 0; i < foodLimit; i++)
        {
            AddFood();
            AddFood(2);
            AddFood(3);
            AddFood(1);
            AddFood(5);
        }

        grid.CreateGrid();

        //Add Player
        int index = UnityEngine.Random.Range(0, grid.walkableNodes.Count);
        Node node = grid.walkableNodes[index];
        player = Instantiate(bubble, node.worldPosition, Quaternion.identity) as GameObject;
        player.GetComponent<SpriteRenderer>().color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        player.transform.localScale = new Vector3(1.8f, 1.8f);
        BasePlayer playerScript = player.AddComponent<Player>();
        player.name = "player1";
        player.tag = "Player";

        ////Spawn AI
        //for (int i = 0; i < aiCount; i++) {
        //	AddAI ();
        //}


        setup = false;



	}
    //public void AddAI (){
    //	int index = UnityEngine.Random.Range (0, spawnLocations.Count);
    //	Coord location = spawnLocations [index];
    //	GameObject ai = Instantiate (bubble, new Vector3 (location.x, location.y), Quaternion.identity) as GameObject;
    //	ai.GetComponent<SpriteRenderer> ().color = new Color (UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    //	ai.transform.localScale = new Vector3(1.8f,1.8f);
    //	ai.name = "AI";
    //	ai.tag = "ai";
    //	//ai.AddComponent<Rigidbody2D> ();
    //	AI aiScript = ai.AddComponent<AI>();
    //	//ai.transform.SetParent (foodEmpty.transform);
    //}
    private void AddFood()
    {
        int index = UnityEngine.Random.Range(0, grid.walkableNodes.Count);
        int foodSize = UnityEngine.Random.Range(minFoodSize, maxFoodSize);
        if (foodSize > highestFoodCount)
        {
            highestFoodCount = foodSize;
        }
        Node node = grid.walkableNodes[index];
        GameObject bubbleFood = Instantiate(food, node.worldPosition, Quaternion.identity) as GameObject;
        bubbleFood.GetComponent<SpriteRenderer>().color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        bubbleFood.transform.localScale = new Vector3(.56f, .56f);
        BasePlayer bubbleFoodScript = bubbleFood.AddComponent<BasePlayer>();
        bubbleFoodScript.food = foodSize;
        bubbleFoodScript.UpdateSize();
        bubbleFood.transform.SetParent(foodEmpty.transform);
    }
    public void AddFood(int size)
    {
        int index = UnityEngine.Random.Range(0, grid.walkableNodes.Count);
        int foodSize = size;
        if (foodSize > highestFoodCount)
        {
            highestFoodCount = foodSize;
        }
        Node node = grid.walkableNodes[index];
        GameObject bubbleFood = Instantiate(food, node.worldPosition, Quaternion.identity) as GameObject;
        bubbleFood.GetComponent<SpriteRenderer>().color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        bubbleFood.transform.localScale = new Vector3(.56f, .56f);
        BasePlayer bubbleFoodScript = bubbleFood.AddComponent<BasePlayer>();
        bubbleFoodScript.food = foodSize;
        bubbleFoodScript.UpdateSize();
        bubbleFood.transform.SetParent(foodEmpty.transform);
    }

   

    private void Timer()
    {
        timer += Time.deltaTime;
        gameTimer.text = Mathf.Round(timer).ToString()+ " s";
    }
    [Serializable]
    private class Scorer
    {
        internal string name;
        internal int time;

        public Scorer(string _name, int _time)
        {
            name = _name;
            time = _time;
        }
        public Scorer()
        {
            name = "ABC";
            time = 10000;
        }
    }
}
