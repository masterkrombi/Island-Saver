using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatTracker : MonoBehaviour
{

    public static StatTracker STATTRACKER;

    string beachScene = "Beach";
    string runnerScene = "EndlessRunnerMode";


    public float distanceTraveled = 0f;
    public float boostDistance = 0f;
    public float kalaniDistance = 0f;
    public float leonaDistance = 0f;
    public float malekoDistance = 0f;
    public float malekoCeilingDistance = 0f;

    public float timeRan = 0f;

    public int treasureCollected = 0;
    public int magnetTreasure = 0;

    bool kalani = false;
    bool maleko = false;
    bool leona = false;
    bool fancy = false;
    bool pirate = false;
    bool geek = false;

    /* For data collection */

    /*
    public float flyerTime = 0f;
    public float jumperTime = 0f;
    public float swapperTime = 0f;
    public int numMonsters;
    public float timeLasted = 0f;

    bool wroteToFile;
    */
    private void Awake()
    {
        if (STATTRACKER == null)
        {
            STATTRACKER = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        if (EndlessRunnerManager.ENDLESSRUNNERMANAGER != null && EndlessRunnerManager.AllDead() == false)
        {
            distanceTraveled += SpeedUpOverTime.speed * Time.deltaTime;
            timeRan += Time.deltaTime;

            if (SpeedUpOverTime.instance.boost)
            {
                boostDistance += SpeedUpOverTime.speed * Time.deltaTime;
            }
            GameObject creature = EndlessRunnerManager.GetActiveCreature();
            Creature activeCreature;
            if (creature != null)
            {
                activeCreature = EndlessRunnerManager.GetActiveCreature().GetComponent<Creature>();
                switch (activeCreature.creatureName)
                {
                    case "Flyer":
                        kalani = true;
                        kalaniDistance += SpeedUpOverTime.speed * Time.deltaTime;
                        break;
                    case "Jumper":
                        leona = true;
                        leonaDistance += SpeedUpOverTime.speed * Time.deltaTime;
                        break;
                    case "Swapper":
                        maleko = true;
                        if (creature.GetComponent<EndlessPlayer>().hasTouchedCeiling)
                        {
                            malekoCeilingDistance += SpeedUpOverTime.speed * Time.deltaTime;
                        }
                        malekoDistance += SpeedUpOverTime.speed * Time.deltaTime;
                        break;
                }
            }

            /* For data collection */

            /*timeLasted += Time.deltaTime;
            if (EndlessRunnerManager.GetActiveCreature() != null)
            {
                if (EndlessRunnerManager.GetActiveCreature().GetComponent<Creature>().creatureName == "Flyer")
                {
                    flyerTime += Time.deltaTime;
                }
                else if (EndlessRunnerManager.GetActiveCreature().GetComponent<Creature>().creatureName == "Jumper")
                {
                    jumperTime += Time.deltaTime;
                }
                else if (EndlessRunnerManager.GetActiveCreature().GetComponent<Creature>().creatureName == "Swapper")
                {
                    swapperTime += Time.deltaTime;
                }
            }
            

            if (EndlessRunnerManager.AllDead() && !wroteToFile)
            {
                DataCollector.instance.WriteToFile(numMonsters, timeLasted, flyerTime, jumperTime, swapperTime, treasureCollected);
                wroteToFile = true;
            }*/
        }
    }

    public void AddTreasure(int amount)
    {
        treasureCollected += amount;
        if (EndlessRunnerManager.GetActiveCreature().GetComponent<EndlessPlayer>().isMagnet)
        {
            magnetTreasure += amount;
        }
    }

    public void ReplayLevel() 
    {
        //Save all data required to save
        //ChallengeSystem.CollectData(distanceTraveled, boostDistance,
        //                            kalaniDistance, leonaDistance, malekoDistance, malekoCeilingDistance,
        //                            treasureCollected, magnetTreasure,
        //                            kalani, maleko, leona,
        //                            fancy, pirate, geek);

        //PlayerManager.CalculateInterest(distanceTraveled);
        PlayerManager.PLAYERMANAGER.gold += treasureCollected;
       
        //Set values back to 0 before loading back into runner
        distanceTraveled = 0f;
        boostDistance = 0f;
        kalaniDistance = 0f;
        leonaDistance = 0f;
        malekoDistance = 0f;
        malekoCeilingDistance = 0f;
        treasureCollected = 0;
        magnetTreasure = 0;
        timeRan = 0f;
    }
    public void CollectData()
    {
        ChallengeSystem.CollectData(distanceTraveled, boostDistance,
                                   kalaniDistance, leonaDistance, malekoDistance, malekoCeilingDistance,
                                   treasureCollected, magnetTreasure,
                                   kalani, maleko, leona,
                                   fancy, pirate, geek);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == beachScene)
        {
            //Save back all data
            //ChallengeSystem.CollectData(distanceTraveled, boostDistance,
            //                            kalaniDistance, leonaDistance, malekoDistance, malekoCeilingDistance,
             //                           treasureCollected, magnetTreasure,
             //                           kalani, maleko, leona,
            //                            fancy, pirate, geek);

            //PlayerManager.CalculateInterest(distanceTraveled);
            PlayerManager.PLAYERMANAGER.gold += treasureCollected;
            PlayerManager.PLAYERMANAGER.UpdateTreasureDisplay();
            PlayerManager.PLAYERMANAGER.UpdateGoldDisplay();

            
            
            //wroteToFile = false;

        }
        else if (scene.name == runnerScene)
        {
            //Zero all data
            distanceTraveled = 0;
            boostDistance = 0;
            kalaniDistance = 0;
            leonaDistance = 0;
            malekoDistance = 0;

            treasureCollected = 0;
            magnetTreasure = 0;

            timeRan = 0;

            PlayerManager _pm = PlayerManager.PLAYERMANAGER;
            if (kalani && maleko && leona)
            {
                if (_pm.flyerAppliedIndices[0] == _pm.jumperAppliedIndices[0] && _pm.flyerAppliedIndices[0] == _pm.swapperAppliedIndices[0])
                {
                    switch (_pm.hats[_pm.flyerAppliedIndices[0]].itemName)
                    {
                        case "Captain's Hat":
                            if (_pm.flyerAppliedIndices[1] == _pm.jumperAppliedIndices[1] && _pm.flyerAppliedIndices[1] == _pm.swapperAppliedIndices[1])
                            {
                                if (_pm.glasses[_pm.flyerAppliedIndices[1]].itemName == "Eyepatch")
                                {
                                    pirate = true;
                                }
                            }
                            break;
                        case "Propellor Hat":
                            if (_pm.flyerAppliedIndices[1] == _pm.jumperAppliedIndices[1] && _pm.flyerAppliedIndices[1] == _pm.swapperAppliedIndices[1])
                            {
                                if (_pm.glasses[_pm.flyerAppliedIndices[1]].itemName == "Geek Glasses")
                                {
                                    geek = true;
                                }
                            }
                            break;
                        case "Top Hat":
                            if (_pm.flyerAppliedIndices[1] == _pm.jumperAppliedIndices[1] && _pm.flyerAppliedIndices[1] == _pm.swapperAppliedIndices[1])
                            {
                                if (_pm.glasses[_pm.flyerAppliedIndices[1]].itemName == "Monocle")
                                {
                                    fancy = true;
                                }
                            }
                            break;
                    }
                }
            }
            else if (kalani && maleko)
            {
                if (_pm.flyerAppliedIndices[0] == _pm.swapperAppliedIndices[0])
                {
                    switch (_pm.hats[_pm.flyerAppliedIndices[0]].itemName)
                    {
                        case "Captain's Hat":
                            if (_pm.flyerAppliedIndices[1] == _pm.swapperAppliedIndices[1])
                            {
                                if (_pm.glasses[_pm.flyerAppliedIndices[1]].itemName == "Eyepatch")
                                {
                                    pirate = true;
                                }
                            }
                            break;
                        case "Propellor Hat":
                            if (_pm.flyerAppliedIndices[1] == _pm.swapperAppliedIndices[1])
                            {
                                if (_pm.glasses[_pm.flyerAppliedIndices[1]].itemName == "Geek Glasses")
                                {
                                    geek = true;
                                }
                            }
                            break;
                        case "Top Hat":
                            if (_pm.flyerAppliedIndices[1] == _pm.swapperAppliedIndices[1])
                            {
                                if (_pm.glasses[_pm.flyerAppliedIndices[1]].itemName == "Monocle")
                                {
                                    fancy = true;
                                }
                            }
                            break;
                    }
                }
            }
            else if (kalani && leona)
            {
                if (_pm.flyerAppliedIndices[0] == _pm.jumperAppliedIndices[0])
                {
                    switch (_pm.hats[_pm.flyerAppliedIndices[0]].itemName)
                    {
                        case "Captain's Hat":
                            if (_pm.flyerAppliedIndices[1] == _pm.jumperAppliedIndices[1])
                            {
                                if (_pm.glasses[_pm.flyerAppliedIndices[1]].itemName == "Eyepatch")
                                {
                                    pirate = true;
                                }
                            }
                            break;
                        case "Propellor Hat":
                            if (_pm.flyerAppliedIndices[1] == _pm.jumperAppliedIndices[1])
                            {
                                if (_pm.glasses[_pm.flyerAppliedIndices[1]].itemName == "Geek Glasses")
                                {
                                    geek = true;
                                }
                            }
                            break;
                        case "Top Hat":
                            if (_pm.flyerAppliedIndices[1] == _pm.jumperAppliedIndices[1])
                            {
                                if (_pm.glasses[_pm.flyerAppliedIndices[1]].itemName == "Monocle")
                                {
                                    fancy = true;
                                }
                            }
                            break;
                    }
                }
            }
            else if (maleko && leona)
            {
                if (_pm.swapperAppliedIndices[0] == _pm.jumperAppliedIndices[0])
                {
                    switch (_pm.hats[_pm.swapperAppliedIndices[0]].itemName)
                    {
                        case "Captain's Hat":
                            if (_pm.swapperAppliedIndices[1] == _pm.jumperAppliedIndices[1])
                            {
                                if (_pm.glasses[_pm.swapperAppliedIndices[1]].itemName == "Eyepatch")
                                {
                                    pirate = true;
                                }
                            }
                            break;
                        case "Propellor Hat":
                            if (_pm.swapperAppliedIndices[1] == _pm.jumperAppliedIndices[1])
                            {
                                if (_pm.glasses[_pm.swapperAppliedIndices[1]].itemName == "Geek Glasses")
                                {
                                    geek = true;
                                }
                            }
                            break;
                        case "Top Hat":
                            if (_pm.swapperAppliedIndices[1] == _pm.jumperAppliedIndices[1])
                            {
                                if (_pm.glasses[_pm.swapperAppliedIndices[1]].itemName == "Monocle")
                                {
                                    fancy = true;
                                }
                            }
                            break;
                    }
                }
            }
            else if (kalani)
            {
                switch (_pm.hats[_pm.flyerAppliedIndices[0]].itemName)
                {
                    case "Captain's Hat":
                        if (_pm.glasses[_pm.flyerAppliedIndices[1]].itemName == "Eyepatch")
                        {
                            pirate = true;
                        }
                        break;
                    case "Propellor Hat":
                        if (_pm.glasses[_pm.flyerAppliedIndices[1]].itemName == "Geek Glasses")
                        {
                            geek = true;
                        }
                        break;
                    case "Top Hat":
                        if (_pm.glasses[_pm.flyerAppliedIndices[1]].itemName == "Monocle")
                        {
                            fancy = true;
                        }
                        break;
                }
            }
            else if (maleko)
            {
                switch (_pm.hats[_pm.swapperAppliedIndices[0]].itemName)
                {
                    case "Captain's Hat":
                        if (_pm.glasses[_pm.swapperAppliedIndices[1]].itemName == "Eyepatch")
                        {
                            pirate = true;
                        }
                        break;
                    case "Propellor Hat":
                        if (_pm.glasses[_pm.swapperAppliedIndices[1]].itemName == "Geek Glasses")
                        {
                            geek = true;
                        }
                        break;
                    case "Top Hat":
                        if (_pm.glasses[_pm.swapperAppliedIndices[1]].itemName == "Monocle")
                        {
                            fancy = true;
                        }
                        break;
                }
            }
            else if (leona)
            {
                switch (_pm.hats[_pm.jumperAppliedIndices[0]].itemName)
                {
                    case "Captain's Hat":
                        if (_pm.glasses[_pm.jumperAppliedIndices[1]].itemName == "Eyepatch")
                        {
                            pirate = true;
                        }
                        break;
                    case "Propellor Hat":
                        if (_pm.glasses[_pm.jumperAppliedIndices[1]].itemName == "Geek Glasses")
                        {
                            geek = true;
                        }
                        break;
                    case "Top Hat":
                        if (_pm.glasses[_pm.jumperAppliedIndices[1]].itemName == "Monocle")
                        {
                            fancy = true;
                        }
                        break;
                }
            }
        }
    }
}
