using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    static public PlayerManager PLAYERMANAGER;

    [Header("Egg Items to get still maybe not need")]
    public List<GameObject> eggMonsterPrefabs;
    public List<GameObject> eggAccessoryPrefabs;
    //public List<GameObject> eggColorPrefabs;
    [SerializeField]
    public List<CustomizationItem> hats;
    [SerializeField]
    public List<CustomizationItem> glasses;
    [SerializeField]
    public List<CustomizationItem> props;
    [SerializeField]
    public List<CustomizationItem> flyerColors;
    [SerializeField]
    public List<CustomizationItem> jumperColors;
    [SerializeField]
    public List<CustomizationItem> swapperColors;

    [SerializeField]
    public List<GameObject> orderedItems;

    public List<int> flyerAppliedIndices;
    public List<int> jumperAppliedIndices;
    public List<int> swapperAppliedIndices;

    public GameObject emptyItem;

    public Text text;
    public Text treasureText;
    public Text progressText;
    public int gold;
    public int treasure;
    public int cocomonks;
    public int overallProgress = 0;

    [Range(0,1)]
    public float baseInterestMultiplier = 0f;
    public float onePercentPerMeter = 250f;
    public float interestAmount;
    public float newTreasure;
    public float previousTreasure;

    public bool loadedAppliedIndices = false;
    public bool showRunnerInstructions = true;

    string beachScene = "Beach";
    Transform propHolder;

	// Use this for initialization
	void Awake()
    {
        if (PLAYERMANAGER == null)
        {
            PLAYERMANAGER = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    /*void Start()
    {

        if (!loadedAppliedIndices)
        {
            //Initilialize all indices lists
            //Index 0 == hat
            //Index 1 == glasses
            //Index 2 == powerup
            //Index 3 == color
            for (int i = 0; i < 4; i++)
            {
                flyerAppliedIndices.Add(0);
                jumperAppliedIndices.Add(0);
                swapperAppliedIndices.Add(0);
            }
            loadedAppliedIndices = true;
        }

        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.name);
        if (currentScene.name == "Beach") 
        {
            Debug.Log("update displays");
            // Gold and Treasure text should initially referenced and updated
            UpdateGoldDisplay();
            UpdateTreasureDisplay();
        }
    }*/

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        if (!loadedAppliedIndices)
        {
            //Initilialize all indices lists
            //Index 0 == hat
            //Index 1 == glasses
            //Index 2 == color
            for (int i = 0; i < 3; i++)
            {
                flyerAppliedIndices.Add(0);
                jumperAppliedIndices.Add(0);
                swapperAppliedIndices.Add(0);
            }
            loadedAppliedIndices = true;
        }

        if (scene.name == beachScene) 
        {
            UpdateGoldDisplay();
            UpdateTreasureDisplay();
            UpdateProgressDisplay();
            if (!SaveLoadManager.FirstLoad)
            {
                SaveLoadManager.instance.SaveAll();
            }

            //Get prop holder transform if a player adds props to beach while in scene
            propHolder = GameObject.Find("Props").transform;
            foreach (CustomizationItem prop in props) 
            {
                for (int i = 0; i < propHolder.childCount; i++) 
                {
                    if (prop.itemName == propHolder.GetChild(i).name)
                        propHolder.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }


    GameObject FindGameObject(CustomizationItem item, List<GameObject> gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.GetComponent<CustomizationItem>().itemName == item.itemName)
            {
                return gameObject;
            }
        }
        return null;
    }
    
    

    //Add item to proper list
    public void AddItem(CustomizationItem item)
    {
        switch (item.type)
        {
            case ItemType.Accessory:
                switch (item.accessoryType)
                {
                    case AccessoryType.Hat:
                        hats.Add(item);
                        break;
                    case AccessoryType.Glasses:
                        glasses.Add(item);
                        break;
                }
                break;
            case ItemType.Color:

                switch (item.creatureType)
                {
                    case CreatureType.Flyer:
                        flyerColors.Add(item);
                        break;
                    case CreatureType.Jumper:
                        jumperColors.Add(item);
                        break;
                    case CreatureType.Swapper:
                        swapperColors.Add(item);
                        break;
                }
                break;
            case ItemType.Prop:
                props.Add(item);
                for (int i = 0; i < propHolder.childCount; i++)
                {
                    GameObject prop = propHolder.GetChild(i).gameObject;
                    if (prop.name == item.itemName)
                    {
                        prop.SetActive(true);
                    }
                }
                break;
        }
    }

    //Pay X gold
    public bool Pay(int amount)
    {
        if (amount > gold)
        {
            return false;
        }

        gold -= amount;
        UpdateGoldDisplay();
        return true;
    }


    public void UpdateGoldDisplay()
    {
        if (text == null)
        {
            text = GameObject.Find("Money Text").GetComponent<Text>();
        }
        text.text = gold.ToString();
    }

    public void UpdateTreasureDisplay() 
    {
        if (treasureText == null)
        {
            treasureText = GameObject.Find("Treasure Text").GetComponent<Text>();
        }
        treasureText.text = treasure.ToString();
    }

    public void UpdateProgressDisplay()
    {
        OverallProgress.PROGRESSMANAGER.CalculateProgress(cocomonks,treasure,CreatureLoader.CREATURELOADER.creaturesToLoadInBeach.Count);
        if (progressText == null)
        {
            progressText = GameObject.Find("PercentText").GetComponent<Text>();
        }
        progressText.text = OverallProgress.PROGRESSMANAGER.GetProgress().ToString()+"%";
    }


    static public void CalculateInterest(float distance)
    {
        //Calculate interest to be applied to treasure in savings
        float remainder = distance % PLAYERMANAGER.onePercentPerMeter;
        distance -= remainder;
        float distanceAdditive = (distance / PLAYERMANAGER.onePercentPerMeter) / 100;
        PLAYERMANAGER.interestAmount = 1 + (PLAYERMANAGER.baseInterestMultiplier + distanceAdditive);

        //Gain interest and store previous for post run summary
        PLAYERMANAGER.previousTreasure = PLAYERMANAGER.treasure;
        PLAYERMANAGER.newTreasure = PLAYERMANAGER.previousTreasure * PLAYERMANAGER.interestAmount;
        PLAYERMANAGER.treasure = (int)PLAYERMANAGER.newTreasure;
    }

    static public void LoadItems(List<CustomizationItem> hats, List<CustomizationItem> glasses, List<CustomizationItem> flyerColors, List<CustomizationItem> jumperColors, List<CustomizationItem> swapperColors, List<CustomizationItem> props)
    {
        PLAYERMANAGER.hats = new List<CustomizationItem>();
        PLAYERMANAGER.glasses = new List<CustomizationItem>();
        PLAYERMANAGER.flyerColors = new List<CustomizationItem>();
        PLAYERMANAGER.jumperColors = new List<CustomizationItem>();
        PLAYERMANAGER.swapperColors = new List<CustomizationItem>();
        PLAYERMANAGER.props = new List<CustomizationItem>();

        foreach (CustomizationItem hat in hats)
        {
            PLAYERMANAGER.AddItem(hat);
        }
        foreach (CustomizationItem prop in props)
        {
            PLAYERMANAGER.AddItem(prop);
        }
        foreach (CustomizationItem eyewear in glasses)
        {
            PLAYERMANAGER.AddItem(eyewear);
        }
        foreach (CustomizationItem color in flyerColors)
        {
            PLAYERMANAGER.AddItem(color);
        }
        foreach (CustomizationItem color in jumperColors)
        {
            PLAYERMANAGER.AddItem(color);
        }
        foreach (CustomizationItem color in swapperColors)
        {
            PLAYERMANAGER.AddItem(color);
        }
    }

    static public void LoadTreasure(int treasure, int gold, int cocomonks)
    {
        PLAYERMANAGER.cocomonks = cocomonks;
        PLAYERMANAGER.gold = gold;
        PLAYERMANAGER.treasure = treasure;
    }

    static public void LoadAppliedIndices(List<int> flyerIndices, List<int> jumperIndices, List<int> swapperIndices)
    {
        
        PLAYERMANAGER.flyerAppliedIndices = new List<int>();
        PLAYERMANAGER.jumperAppliedIndices = new List<int>();
        PLAYERMANAGER.swapperAppliedIndices = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            PLAYERMANAGER.flyerAppliedIndices.Add(flyerIndices[i]);
            PLAYERMANAGER.jumperAppliedIndices.Add(jumperIndices[i]);
            PLAYERMANAGER.swapperAppliedIndices.Add(swapperIndices[i]);
        }

        PLAYERMANAGER.loadedAppliedIndices = true;
        
    }
}