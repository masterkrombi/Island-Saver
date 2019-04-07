using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum RewardType
{
    Monetary,
    Clam,
    Creature
}

[System.Serializable]
public enum ChallengeType
{
    None,
    Run,
    Treasure,
    Distance,
    NotChallenge
}

[System.Serializable]
public enum SpecialChallengeType
{
    None,
    Pirate,
    Fancy,
    Geek,
    Singular,
    Boost,
    Magnet
}
[System.Serializable]
public class ChallengeSystem : MonoBehaviour {

    static public ChallengeSystem CHALLENGESYSTEM;

    [Header("These are all the challenges in the game")]
    [SerializeField]public List<Challenge> challengePool;

    [Header("Challenges for first displaying monsters")]
    public List<Challenge> monsterChallenges;
    public List<Challenge> lastTwoMonsters;

    [Header("Only use this challenge for tutorial")]
    public Challenge tutorialChallenge;

    [Header("Use this challenge when user is out of challenges")]
    public Challenge noMoreChallenge;

    [Header("List of challenges currently active")]
    [SerializeField]public List<Challenge> activeChallenges;

    [Header("Completed Challenges")]
    [SerializeField]public List<Challenge> completedChallenges;

    [Header("Rewards for Challenges")]
    [SerializeField] public List<GameObject> rewards;

    public int maxChallenges = 3;
    List<Challenge> challengesToShow;
    public List<Challenge> recycledChallenges;

    private bool firstChallengeSelected;
    public bool firstMonstersSelected;

    public bool FirstChallengeSelected{
        get
        {
            return firstChallengeSelected;
        }
        set{
            firstChallengeSelected = value;
        }
    }


    private void Awake()
    {
        if (CHALLENGESYSTEM == null)
        {
            CHALLENGESYSTEM = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        challengesToShow = new List<Challenge>();

       
    }

    public void InitializeChallenges()
    {


        for (int i = 0; i < maxChallenges; i++)
        {
            activeChallenges.Add(null);
            SelectNewChallenge(i);
        }
    }

    bool CheckForNoneChallenges(List<Challenge> challengesList) 
    {
        for (int i = 0; i < challengesList.Count; i++) 
        {
            if (challengesList[i].creatureType == CreatureType.None) return true;
        }
        return false;
    }

    public static void CollectData(float distance, float boostDistance,
                                   float kalaniDistance, float leonaDistance, float malekoDistance, float malekoCeilingDistance,
                                   int treasure, int magnetTreasure,
                                   bool kalani, bool maleko, bool leona,
                                   bool fancy, bool pirate, bool geek)
    {
        foreach (Challenge challenge in CHALLENGESYSTEM.activeChallenges)
        {
            switch (challenge.specialType)
            {
                case SpecialChallengeType.None:
                    switch (challenge.creatureType)
                    {
                        case CreatureType.None:
                            switch (challenge.challengeType)
                            {
                                case ChallengeType.Run:
                                    break;
                                case ChallengeType.Distance:
                                    challenge.value += distance;
                                    if (challenge.value >= challenge.achieveValue)
                                    {
                                        challenge.value = challenge.achieveValue;
                                        challenge.done = true;
                                    }
                                    break;
                                case ChallengeType.Treasure:
                                    challenge.value += treasure;
                                    if (challenge.value > challenge.achieveValue)
                                    {
                                        challenge.value = challenge.achieveValue;
                                        challenge.done = true;
                                    }
                                    break;
                            }
                            break;
                        case CreatureType.Flyer:
                            switch (challenge.challengeType)
                            {
                                case ChallengeType.Run:
                                    if (kalani)
                                    {
                                        challenge.value++;
                                        if (challenge.value >= challenge.achieveValue)
                                        {
                                            challenge.value = challenge.achieveValue;
                                            challenge.done = true;
                                        }
                                    }
                                    break;
                                case ChallengeType.Distance:
                                    challenge.value += kalaniDistance;
                                    if (challenge.value >= challenge.achieveValue)
                                    {
                                        challenge.value = challenge.achieveValue;
                                        challenge.done = true;
                                    }
                                    break;
                                case ChallengeType.Treasure:
                                    
                                    break;
                            }
                            break;
                        case CreatureType.Jumper:
                            switch (challenge.challengeType)
                            {
                                case ChallengeType.Run:
                                    if (leona)
                                    {
                                        challenge.value++;
                                        if (challenge.value >= challenge.achieveValue)
                                        {
                                            challenge.value = challenge.achieveValue;
                                            challenge.done = true;
                                        }
                                    }
                                    break;
                                case ChallengeType.Distance:
                                    challenge.value += leonaDistance;
                                    if (challenge.value > challenge.achieveValue)
                                    {
                                        challenge.value = challenge.achieveValue;
                                        challenge.done = true;
                                    }
                                    break;
                                case ChallengeType.Treasure:
                                    
                                    break;
                            }
                            break;
                        case CreatureType.Swapper:
                            switch (challenge.challengeType)
                            {
                                case ChallengeType.Run:
                                    if (maleko)
                                    {
                                        challenge.value++;
                                        if (challenge.value >= challenge.achieveValue)
                                        {
                                            challenge.value = challenge.achieveValue;
                                            challenge.done = true;
                                        }
                                    }
                                    break;
                                case ChallengeType.Distance:
                                    challenge.value += malekoDistance;
                                    if (challenge.value > challenge.achieveValue)
                                    {
                                        challenge.value = challenge.achieveValue;
                                        challenge.done = true;
                                    }
                                    break;
                                case ChallengeType.Treasure:
                                    
                                    break;
                            }
                            break;
                    }
                    break;
                case SpecialChallengeType.Fancy:
                    if (fancy)
                    {
                        challenge.value++;
                        if (challenge.value >= challenge.achieveValue)
                        {
                            challenge.value = challenge.achieveValue;
                            challenge.done = true;
                        }
                    }
                    break;
                case SpecialChallengeType.Pirate:
                    if (pirate)
                    {
                        challenge.value++;
                        if (challenge.value >= challenge.achieveValue)
                        {
                            challenge.value = challenge.achieveValue;
                            challenge.done = true;
                        }
                    }
                    break;
                case SpecialChallengeType.Geek:
                    if (geek)
                    {
                        challenge.value++;
                        if (challenge.value >= challenge.achieveValue)
                        {
                            challenge.value = challenge.achieveValue;
                            challenge.done = true;
                        }
                    }
                    break;
                case SpecialChallengeType.Singular:
                    switch (challenge.challengeType) 
                    {
                        case ChallengeType.Distance:
                            switch (challenge.creatureType)
                            {
                                case CreatureType.Flyer:
                                    break;
                                case CreatureType.Jumper:
                                    break;
                                case CreatureType.Swapper:
                                    challenge.value += malekoCeilingDistance;
                                    break;
                                case CreatureType.None:
                                    challenge.value += distance;
                                    break;
                            }
                            break;
                        case ChallengeType.Treasure:
                            challenge.value += treasure;
                            break;
                    }

                    if (challenge.value >= challenge.achieveValue) 
                    {
                        challenge.value = challenge.achieveValue;
                        challenge.done = true;
                    }

                    if (!challenge.done) 
                    {
                        challenge.value = 0;
                    }   

                    break;
                case SpecialChallengeType.Boost:
                    switch (challenge.creatureType)
                    {
                        case CreatureType.None:
                            switch (challenge.challengeType)
                            {
                                case ChallengeType.Run:
                                    break;
                                case ChallengeType.Distance:
                                    challenge.value += boostDistance;
                                    if (challenge.value > challenge.achieveValue)
                                    {
                                        challenge.value = challenge.achieveValue;
                                        challenge.done = true;
                                    }
                                    break;
                                case ChallengeType.Treasure:
                                    
                                    break;
                            }
                            break;
                        case CreatureType.Flyer:
                            switch (challenge.challengeType)
                            {
                                case ChallengeType.Run:
                                    break;
                                case ChallengeType.Distance:
                                    
                                    break;
                                case ChallengeType.Treasure:

                                    break;
                            }
                            break;
                        case CreatureType.Jumper:
                            switch (challenge.challengeType)
                            {
                                case ChallengeType.Run:
                                    break;
                                case ChallengeType.Distance:
                                    
                                    break;
                                case ChallengeType.Treasure:

                                    break;
                            }
                            break;
                        case CreatureType.Swapper:
                            switch (challenge.challengeType)
                            {
                                case ChallengeType.Run:
                                    break;
                                case ChallengeType.Distance:
                                    
                                    break;
                                case ChallengeType.Treasure:

                                    break;
                            }
                            break;
                    }
                    break;
                case SpecialChallengeType.Magnet:
                    switch (challenge.creatureType)
                    {
                        case CreatureType.None:
                            switch (challenge.challengeType)
                            {
                                case ChallengeType.Run:
                                    break;
                                case ChallengeType.Distance:
                                    
                                    break;
                                case ChallengeType.Treasure:
                                    challenge.value += magnetTreasure;
                                    if (challenge.value > challenge.achieveValue)
                                    {
                                        challenge.value = challenge.achieveValue;
                                        challenge.done = true;
                                    }
                                    break;
                            }
                            break;
                        case CreatureType.Flyer:
                            switch (challenge.challengeType)
                            {
                                case ChallengeType.Run:
                                    break;
                                case ChallengeType.Distance:
                                    
                                    break;
                                case ChallengeType.Treasure:

                                    break;
                            }
                            break;
                        case CreatureType.Jumper:
                            switch (challenge.challengeType)
                            {
                                case ChallengeType.Run:
                                    break;
                                case ChallengeType.Distance:
                                    
                                    break;
                                case ChallengeType.Treasure:

                                    break;
                            }
                            break;
                        case CreatureType.Swapper:
                            switch (challenge.challengeType)
                            {
                                case ChallengeType.Run:
                                    break;
                                case ChallengeType.Distance:
                                    
                                    break;
                                case ChallengeType.Treasure:

                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }

    static public void SelectNewChallenge(int index)
    {
        //Add to completed or recycled challenges based on completion status
        if (CHALLENGESYSTEM.activeChallenges[index] != null) 
        {
            if (CHALLENGESYSTEM.activeChallenges[index].done)
            {
                CHALLENGESYSTEM.completedChallenges.Add(CHALLENGESYSTEM.activeChallenges[index]);
            }
            else
            {
                CHALLENGESYSTEM.recycledChallenges.Add(CHALLENGESYSTEM.activeChallenges[index]);
            }    
        }

        //Add new challenge
        //If tutorial is in progress, add tutorial challenge
        //Otherwise add challenge as long as it doesn't involve monster player doesn't have
        //If no 'none' challenges are available instead, add challenge anyways
        //Go through challenge pool first then recycled pool next
        if (TutorialTracker.tutorialInProgress && !CHALLENGESYSTEM.firstChallengeSelected) 
        {
            CHALLENGESYSTEM.activeChallenges[index] = CHALLENGESYSTEM.tutorialChallenge;
            CHALLENGESYSTEM.firstChallengeSelected = true;
        }
        else if (CHALLENGESYSTEM.challengePool.Count > 0 || CHALLENGESYSTEM.recycledChallenges.Count > 0)
        {
            if (CHALLENGESYSTEM.challengePool.Count > 0)
            {
                bool replaced = false;
                int poolIndex = Random.Range(0, CHALLENGESYSTEM.challengePool.Count);

                while (replaced == false) 
                {
                    switch (CHALLENGESYSTEM.challengePool[poolIndex].creatureType) 
                    {
                        case CreatureType.None:
                            CHALLENGESYSTEM.activeChallenges[index] = CHALLENGESYSTEM.challengePool[poolIndex];
                            CHALLENGESYSTEM.challengePool.RemoveAt(poolIndex);
                            replaced = true;
                            break;
                        case CreatureType.Flyer:
                            if ((CreatureLoader.CREATURELOADER.creaturesToLoadInBeach.Contains("Flyer")) ||
                                (!CHALLENGESYSTEM.CheckForNoneChallenges(CHALLENGESYSTEM.challengePool))) 
                            {
                                CHALLENGESYSTEM.activeChallenges[index] = CHALLENGESYSTEM.challengePool[poolIndex];
                                CHALLENGESYSTEM.challengePool.RemoveAt(poolIndex);
                                replaced = true;    
                            }
                            else 
                            {
                                poolIndex = Random.Range(0, CHALLENGESYSTEM.challengePool.Count);
                            }

                            break;
                        case CreatureType.Jumper:
                            if ((CreatureLoader.CREATURELOADER.creaturesToLoadInBeach.Contains("Jumper")) ||
                                (!CHALLENGESYSTEM.CheckForNoneChallenges(CHALLENGESYSTEM.challengePool))) 
                            {
                                CHALLENGESYSTEM.activeChallenges[index] = CHALLENGESYSTEM.challengePool[poolIndex];
                                CHALLENGESYSTEM.challengePool.RemoveAt(poolIndex);
                                replaced = true;
                            }
                            else
                            {
                                poolIndex = Random.Range(0, CHALLENGESYSTEM.challengePool.Count);
                            }

                            break;
                        case CreatureType.Swapper:
                            if ((CreatureLoader.CREATURELOADER.creaturesToLoadInBeach.Contains("Swapper")) ||
                                (!CHALLENGESYSTEM.CheckForNoneChallenges(CHALLENGESYSTEM.challengePool))) 
                            {
                                CHALLENGESYSTEM.activeChallenges[index] = CHALLENGESYSTEM.challengePool[poolIndex];
                                CHALLENGESYSTEM.challengePool.RemoveAt(poolIndex);
                                replaced = true;
                            }
                            else
                            {
                                poolIndex = Random.Range(0, CHALLENGESYSTEM.challengePool.Count);
                            }

                            break;
                    }
                }
            }
            else if (CHALLENGESYSTEM.recycledChallenges.Count > 0)
            {
                bool replaced = false;
                int poolIndex = Random.Range(0, CHALLENGESYSTEM.recycledChallenges.Count);

                while (replaced == false)
                {
                    switch (CHALLENGESYSTEM.recycledChallenges[poolIndex].creatureType)
                    {
                        case CreatureType.None:
                            CHALLENGESYSTEM.activeChallenges[index] = CHALLENGESYSTEM.recycledChallenges[poolIndex];
                            CHALLENGESYSTEM.recycledChallenges.RemoveAt(poolIndex);
                            replaced = true;
                            break;
                        case CreatureType.Flyer:
                            if ((CreatureLoader.CREATURELOADER.creaturesToLoadInBeach.Contains("Flyer")) ||
                                (!CHALLENGESYSTEM.CheckForNoneChallenges(CHALLENGESYSTEM.recycledChallenges))) 
                            {
                                CHALLENGESYSTEM.activeChallenges[index] = CHALLENGESYSTEM.recycledChallenges[poolIndex];
                                CHALLENGESYSTEM.recycledChallenges.RemoveAt(poolIndex);
                                replaced = true;
                            }
                            else
                            {
                                poolIndex = Random.Range(0, CHALLENGESYSTEM.recycledChallenges.Count);
                            }

                            break;
                        case CreatureType.Jumper:
                            if ((CreatureLoader.CREATURELOADER.creaturesToLoadInBeach.Contains("Jumper")) ||
                                (!CHALLENGESYSTEM.CheckForNoneChallenges(CHALLENGESYSTEM.recycledChallenges))) 
                            {
                                CHALLENGESYSTEM.activeChallenges[index] = CHALLENGESYSTEM.recycledChallenges[poolIndex];
                                CHALLENGESYSTEM.recycledChallenges.RemoveAt(poolIndex);
                                replaced = true;
                            }
                            else
                            {
                                poolIndex = Random.Range(0, CHALLENGESYSTEM.recycledChallenges.Count);
                            }

                            break;
                        case CreatureType.Swapper:
                            if ((CreatureLoader.CREATURELOADER.creaturesToLoadInBeach.Contains("Swapper")) ||
                                (!CHALLENGESYSTEM.CheckForNoneChallenges(CHALLENGESYSTEM.recycledChallenges))) 
                            {
                                CHALLENGESYSTEM.activeChallenges[index] = CHALLENGESYSTEM.recycledChallenges[poolIndex];
                                CHALLENGESYSTEM.recycledChallenges.RemoveAt(poolIndex);
                                replaced = true;
                            }
                            else
                            {
                                poolIndex = Random.Range(0, CHALLENGESYSTEM.recycledChallenges.Count);
                            }

                            break;
                    }
                }
            }

            //Select reward for challenges that give item clam
            if (CHALLENGESYSTEM.activeChallenges[index].rewardType == RewardType.Clam) 
            {
				//TODO: change to remove challenge from shop, give prefab to Challenge system as reward. 
                //GameObject reward = ShopManager.SelectChallengeReward();
			//print("REWARD " + reward);
                //if (reward != null)
                    //CHALLENGESYSTEM.activeChallenges[index].clamReward =  reward;
            }
        }
        else 
        {
            CHALLENGESYSTEM.activeChallenges[index] = CHALLENGESYSTEM.noMoreChallenge;
        }
    }




    static public void LoadChallenges(List<Challenge> challengePool,List<Challenge> activeChallenges, List<Challenge> completedChallenges)
    {
        CHALLENGESYSTEM.challengePool = new List<Challenge>();
        CHALLENGESYSTEM.activeChallenges = new List<Challenge>();
        CHALLENGESYSTEM.completedChallenges = new List<Challenge>();


        foreach (Challenge challenge in challengePool)
        {
            CHALLENGESYSTEM.challengePool.Add(challenge);
        }
        foreach (Challenge challenge in activeChallenges)
        {
            CHALLENGESYSTEM.activeChallenges.Add(challenge);
        }
        foreach (Challenge challenge in completedChallenges)
        {
            CHALLENGESYSTEM.completedChallenges.Add(challenge);
        }
    }



    [System.Serializable]
    public class Challenge
    {
        public string name;

        [TextArea(4, 10)]
        public string description;
        public float value;
        public float achieveValue;
        public bool done = false;
        public bool displayed = false;
        public bool canRefresh = true;
        public ChallengeType challengeType;
        public SpecialChallengeType specialType;
        public CreatureType creatureType;
        public RewardType rewardType;
        //public GameObject clamReward;
        public int rewardIndex;
        public int monetaryReward;
    }
}
