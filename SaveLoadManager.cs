using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveLoad;

[System.Serializable]
public class SaveLoadManager : MonoBehaviour
{
    static public SaveLoadManager instance;

    //Referances to all objects that need saving or loading on Application open and close
    public List<GameObject> creaturesOwned;
    public int numCreatures;
    public bool resetSaveFile = false;

    public SaveFile save;
    public SaveFile load;

    private static bool initialRun =true;
    private static bool firstLoad = true;

    public void Awake()
    {
        if (initialRun)
        {
            if (!resetSaveFile)
            {
                firstLoad = true;
            }
            else
            {
                firstLoad = false;
            }
           initialRun = false;
        }
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static bool FirstLoad
    {
        get { return firstLoad; }

    }
    void Start ()
    {
        numCreatures = 0;
        if (File.Exists(Application.persistentDataPath + "/SaveFile") && !CreatureLoader.newGame )
        {
            LoadAll();
            PlayerManager.PLAYERMANAGER.UpdateProgressDisplay();
            PlayerManager.PLAYERMANAGER.UpdateGoldDisplay();
            PlayerManager.PLAYERMANAGER.UpdateTreasureDisplay();
           
        }
        firstLoad = false;
       
	}

    //Saves all data, typically called as application quits
    public void SaveAll()
    {
        SaveFile save = new SaveFile();
        
        //CreatureLoader info
        save.creaturesOwnedNames = CreatureLoader.CREATURELOADER.creaturesToLoadInBeach;

        //PlayerManager info
        save.treasure = PlayerManager.PLAYERMANAGER.treasure;
        save.gold = PlayerManager.PLAYERMANAGER.gold;
        save.cocomonks = PlayerManager.PLAYERMANAGER.cocomonks;

        save.hats = PlayerManager.PLAYERMANAGER.hats;
        save.glasses = PlayerManager.PLAYERMANAGER.glasses;
        save.flyerColors = PlayerManager.PLAYERMANAGER.flyerColors;
        save.props = PlayerManager.PLAYERMANAGER.props;
        save.jumperColors = PlayerManager.PLAYERMANAGER.jumperColors;
        save.swapperColors = PlayerManager.PLAYERMANAGER.swapperColors;
       
        save.flyerAppliedIndices = PlayerManager.PLAYERMANAGER.flyerAppliedIndices;
        save.jumperAppliedIndices = PlayerManager.PLAYERMANAGER.jumperAppliedIndices;
        save.swapperAppliedIndices = PlayerManager.PLAYERMANAGER.swapperAppliedIndices;

        //ChallengeSystem info
        save.challengePool = ChallengeSystem.CHALLENGESYSTEM.challengePool;
        save.activeChallenges = ChallengeSystem.CHALLENGESYSTEM.activeChallenges;
        save.completedChallenges = ChallengeSystem.CHALLENGESYSTEM.completedChallenges;
        save.firstRunChallenge = ChallengeSystem.CHALLENGESYSTEM.FirstChallengeSelected;

        
        //Tutorial Tracker
        save.tutorialInProgress = TutorialTracker.tutorialInProgress;
        save.completedFirstRun = TutorialTracker.completedFirstRun;
        save.firstTimeOnBeach = TutorialTracker.firstTimeOnBeach;
        save.firstTimeInChallenges = TutorialTracker.firstTimeInChallenges;
        save.firstTimeInCustomizer = TutorialTracker.firstTimeInCustomizer;
        save.firstTimeInShop = TutorialTracker.firstTimeInShop;
        save.firstTimeInTiki = TutorialTracker.firstTimeInTiki;
        save.completedFirstDialogue = TutorialTracker.firstDialogueDone;

        //Save State manager
        save.gameState = StateManager.STATEMANAGER.currentState;


        if(TikiManager.TIKIMANAGER.unlockableCreatures.Count>0)
            save.clamHatchTime = TikiManager.TIKIMANAGER.unlockableCreatures[0].currentHatchTime;
         

        //Tutorial Manager
        if (TutorialManager.TUTORIALMANAGER.currentStep < TutorialManager.TUTORIALMANAGER.goToCastleStep)
            save.currentStep = 0;
        else if (TutorialManager.TUTORIALMANAGER.currentStep < TutorialManager.TUTORIALMANAGER.chooseMonsterStep)
            save.currentStep = TutorialManager.TUTORIALMANAGER.goToCastleStep;
        else if (TutorialManager.TUTORIALMANAGER.currentStep < TutorialManager.TUTORIALMANAGER.backFromCastleStep)
            save.currentStep = TutorialManager.TUTORIALMANAGER.chooseMonsterStep;
        else if (TutorialManager.TUTORIALMANAGER.currentStep < TutorialManager.TUTORIALMANAGER.departOnRunStep)
            save.currentStep = TutorialManager.TUTORIALMANAGER.backFromCastleStep;
        else if (TutorialManager.TUTORIALMANAGER.currentStep < TutorialManager.TUTORIALMANAGER.claimRewardStep)
            save.currentStep = TutorialManager.TUTORIALMANAGER.departOnRunStep;
        else if (TutorialManager.TUTORIALMANAGER.currentStep < TutorialManager.TUTORIALMANAGER.goToTikiStep)
            save.currentStep = TutorialManager.TUTORIALMANAGER.claimRewardStep;
        else if (TutorialManager.TUTORIALMANAGER.currentStep < TutorialManager.TUTORIALMANAGER.makeWithdrawalStep)
            save.currentStep = TutorialManager.TUTORIALMANAGER.goToTikiStep;
        else if (TutorialManager.TUTORIALMANAGER.currentStep < TutorialManager.TUTORIALMANAGER.goToShopStep)
            save.currentStep = TutorialManager.TUTORIALMANAGER.makeWithdrawalStep;
        else if (TutorialManager.TUTORIALMANAGER.currentStep < TutorialManager.TUTORIALMANAGER.buyItemStep)
            save.currentStep = TutorialManager.TUTORIALMANAGER.goToShopStep;
        else if (TutorialManager.TUTORIALMANAGER.currentStep < TutorialManager.TUTORIALMANAGER.lastStep)
            save.currentStep = TutorialManager.TUTORIALMANAGER.buyItemStep;
        else
            save.currentStep = TutorialManager.TUTORIALMANAGER.lastStep;

        save.currentDialogueSet = TutorialManager.TUTORIALMANAGER.currentDialogueSet;

        Saver.Save(save, "SaveFile");
    }

    //Loads all data, typically called when application starts
    public void LoadAll()
    {
        SaveFile load = Loader.Load("SaveFile");
        
        CreatureLoader.LoadCreatures(load.creaturesOwnedNames);

        PlayerManager.LoadItems(load.hats, 
                                load.glasses, 
                                load.flyerColors, 
                                load.jumperColors, 
                                load.swapperColors, load.props);

        PlayerManager.LoadTreasure(load.treasure, load.gold, load.cocomonks);

        PlayerManager.LoadAppliedIndices(load.flyerAppliedIndices, 
                                         load.jumperAppliedIndices, 
                                         load.swapperAppliedIndices);

        ChallengeSystem.LoadChallenges(load.challengePool,
                                       load.activeChallenges,
                                       load.completedChallenges);

        ChallengeSystem.CHALLENGESYSTEM.FirstChallengeSelected = load.firstRunChallenge;

        TutorialTracker.LoadBools(load.tutorialInProgress, load.completedFirstRun,
            load.firstTimeOnBeach, load.firstTimeInShop, load.firstTimeInChallenges, 
            load.firstTimeInCustomizer, load.firstTimeInTiki, load.completedFirstDialogue);

        TutorialManager.LoadTutorialProgress(load.currentStep, load.currentDialogueSet);

        TikiManager.SetClamTimes(load.clamHatchTime);

        StateManager.LoadState(load.gameState);
    }
}
