using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreatureLoader : MonoBehaviour {
    static public CreatureLoader CREATURELOADER;
    public static bool newGame = false;

    public List<string> creaturesToLoadInBeach;
    public List<string> creaturesToLoadInRunner;
    
    public List<PlacementTile > boatTiles;
    [Header("Creature Prefabs")]
    public List<GameObject> loadableCreatures;

    public List<float> unlockableMonsterHatchTimes;

    GameObject boat;
    string beachScene = "Beach";
    string runnerScene = "EndlessRunnerMode";
    string alexScene = "AlexBeachSandbox";
    bool unlockableMonstersDisplayed;

    public bool cocomonkCollected = false;
    // Use this for initialization
    void Awake ()
    {
        if (CREATURELOADER == null)
        {
            CREATURELOADER = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
        
        boatTiles = new List<PlacementTile>();
    }
  
    //Save creatures to runner loadable list
    public void SaveCreatures()
    {
        foreach (PlacementTile tile in boatTiles)
        {
            foreach (GameObject creature in tile.creatures)
            {
                creaturesToLoadInRunner.Add(creature.GetComponent<Creature>().creatureName);
            }
        }
    }

    //Load from saveloadmanager only
    static public void LoadCreatures(List<string> creatureNames)
    {
        CREATURELOADER.creaturesToLoadInBeach = new List<string>();
        foreach (string name in creatureNames)
        {
            CREATURELOADER.creaturesToLoadInBeach.Add(name);
        }
    }

    static public void AddCreatureToBeach(string beachCreature)
    {
        CREATURELOADER.creaturesToLoadInBeach.Add(beachCreature);
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == CREATURELOADER.beachScene) 
        {
            GameObject go = Instantiate(CREATURELOADER.loadableCreatures[CREATURELOADER.FindIndexOfName(beachCreature + "Beach")]);
            if (!beachCreature.Contains("Cocomonk")) 
            {
                CustomizationMenuManager.CUSTOMIZATIONMENUMANAGER.monsters.Add(go);
            }
        }
    }

    bool runInStart = false;

    //Delegate function for SceneLoaded
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {   
        if (scene.name == beachScene || scene.name == alexScene)
        {
            //If beach scene load all creatures that you have
            boatTiles = new List<PlacementTile>();
            BeachManager beachManager = GameObject.FindGameObjectWithTag("BeachManager").GetComponent<BeachManager>();
            foreach (PlacementTile tile in beachManager.boatTiles)
            {
                boatTiles.Add(tile);
            }

            if (SaveLoadManager.FirstLoad)
            {
                runInStart = true;
            }
            else
            {
                AddCreatures();
            }

            
            //For runner
            bool flyer = false, jumper = false, swapper = false;
            foreach (string name in creaturesToLoadInRunner)
            {
                if (name == "Flyer")
                {
                    flyer = true;
                }
                else if (name == "Jumper")
                {
                    jumper = true;
                }
                else if (name == "Swapper")
                {
                    swapper = true;
                }
            }

           
        }
        else if(scene.name == runnerScene)
        {
            boat = GameObject.Find("Boat");
            //If runner scene load only creature(s) in boat
            for (int i = 0; i < creaturesToLoadInRunner.Count; i++)
            {
                GameObject go = Instantiate(loadableCreatures[FindIndexOfName(creaturesToLoadInRunner[i] + "Runner")]);
                go.transform.position = boat.transform.position + new Vector3 (0, 0, boat.GetComponent<BoxCollider2D>().bounds.max.y);
                EndlessRunnerManager.ENDLESSRUNNERMANAGER.creaturesInBoat.Add(go);
                go.GetComponent<EndlessPlayer>().boatIndex = i;
            }

            boatTiles = new List<PlacementTile>();
        }
    }

    private void Update()
    {
        if (runInStart)
        {
            AddCreatures();
           
            runInStart = false;
            SaveLoadManager.instance.SaveAll();
        }
    }

    void AddCreatures()
    {
        if (creaturesToLoadInBeach != null && creaturesToLoadInBeach.Count > 0)
        {
            for (int i = 0; i < creaturesToLoadInBeach.Count; i++)
            {
                GameObject go = Instantiate(loadableCreatures[FindIndexOfName(creaturesToLoadInBeach[i] + "Beach")]);
                if (!creaturesToLoadInBeach[i].Contains("Cocomonk"))
                {
                    CustomizationMenuManager.CUSTOMIZATIONMENUMANAGER.monsters.Add(go);
                }
            }
            //Add creature clams for creatures you don't have to tiki if not added yet
            if (TikiManager.TIKIMANAGER.unlockableCreatures.Count > 0)
            {
                for (int i = 0; i < creaturesToLoadInBeach.Count; i++)
                {
                    for (int c = 0; c < TikiManager.TIKIMANAGER.unlockableCreatures.Count; c++)
                    {
                        if (TikiManager.TIKIMANAGER.unlockableCreatures[c].creatureName.Equals(creaturesToLoadInBeach[i]))
                        {
                            TikiManager.TIKIMANAGER.unlockableCreatures.Remove(TikiManager.TIKIMANAGER.unlockableCreatures[c]);
                        }
                    }
                }
                TikiManager.TIKIMANAGER.DisplayCreatures();
            }

            creaturesToLoadInRunner.Clear();
        }
        else
        {
            TikiManager.TIKIMANAGER.DisplayFirstCreatures();
        }
    }

    //Add percentages of unlockable creature clams between scenes
    static public void AddToPercentages(int order, float timeToAdd = 0) 
    {
        switch (order) 
        {
            case 0:
                for (int i = 0; i < TikiManager.TIKIMANAGER.unlockableCreatures.Count; i++)
                {
                    CREATURELOADER.unlockableMonsterHatchTimes[i] = TikiManager.TIKIMANAGER.unlockableCreatures[i].currentHatchTime;
                }

                break;
            case 1:
                for (int i = 0; i < CREATURELOADER.unlockableMonsterHatchTimes.Count; i++) 
                {
                    CREATURELOADER.unlockableMonsterHatchTimes[i] += timeToAdd;
                }

                break;
        }

    }

    //Find correct index of gameobject based on the name given and name of the gameobject
    public int FindIndexOfName(string name)
    {
        int index = 0;
        foreach (GameObject creature in loadableCreatures)
        {
            if (creature.name == name)
            {
                break;
            }
            index++;
        }

        return index;
    }
}
