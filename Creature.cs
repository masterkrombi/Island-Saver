using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Creature : MonoBehaviour {
    
    public string creatureName;
    public SkinnedMeshRenderer monsterMesh;
    public GameObject accessoriesEmpty;

    public List<CustomizationItem> equippedItems;

    void Start()
    {
        // For each creature load in there color and accessories
        switch (creatureName)
        {
            case "Flyer":
                if (PlayerManager.PLAYERMANAGER.flyerAppliedIndices[0] > 0)
                {
                    PlayerManager.PLAYERMANAGER.hats[PlayerManager.PLAYERMANAGER.flyerAppliedIndices[0]].ApplyItem(gameObject, false);
                }
                if (PlayerManager.PLAYERMANAGER.flyerAppliedIndices[1] > 0)
                {
                    PlayerManager.PLAYERMANAGER.glasses[PlayerManager.PLAYERMANAGER.flyerAppliedIndices[1]].ApplyItem(gameObject, false);
                }
                if (PlayerManager.PLAYERMANAGER.flyerAppliedIndices[2] != -1)
                {
                    PlayerManager.PLAYERMANAGER.flyerColors[PlayerManager.PLAYERMANAGER.flyerAppliedIndices[2]].ApplyItem(gameObject, false);
                }
                break;
            case "Jumper":
                if (PlayerManager.PLAYERMANAGER.jumperAppliedIndices[0] > 0)
                {
                    PlayerManager.PLAYERMANAGER.hats[PlayerManager.PLAYERMANAGER.jumperAppliedIndices[0]].ApplyItem(gameObject, false);
                }
                if (PlayerManager.PLAYERMANAGER.jumperAppliedIndices[1] > 0)
                {
                    PlayerManager.PLAYERMANAGER.glasses[PlayerManager.PLAYERMANAGER.jumperAppliedIndices[1]].ApplyItem(gameObject, false);
                }
                if (PlayerManager.PLAYERMANAGER.jumperAppliedIndices[2] != -1)
                {
                    PlayerManager.PLAYERMANAGER.jumperColors[PlayerManager.PLAYERMANAGER.jumperAppliedIndices[2]].ApplyItem(gameObject, false);
                }
                break;
            case "Swapper":
                if (PlayerManager.PLAYERMANAGER.swapperAppliedIndices[0] > 0)
                {
                    PlayerManager.PLAYERMANAGER.hats[PlayerManager.PLAYERMANAGER.swapperAppliedIndices[0]].ApplyItem(gameObject, false);
                }
                if (PlayerManager.PLAYERMANAGER.swapperAppliedIndices[1] > 0)
                {
                    PlayerManager.PLAYERMANAGER.glasses[PlayerManager.PLAYERMANAGER.swapperAppliedIndices[1]].ApplyItem(gameObject, false);
                }
                if (PlayerManager.PLAYERMANAGER.swapperAppliedIndices[2] != -1)
                {
                    PlayerManager.PLAYERMANAGER.swapperColors[PlayerManager.PLAYERMANAGER.swapperAppliedIndices[2]].ApplyItem(gameObject, false);
                }
                break;
        }
    }

    public void ChangeColor(List<Material> materialsList, bool _animation = true)
    {
        if (_animation)
        {
            if(!(gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Lizard Pose")|| gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Leo Pose")|| gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Bat Pose")))
            {
                gameObject.GetComponent<Animator>().SetTrigger("Equip");
            }
        }

        BeachAudioManager.PlaySound(BeachSoundType.ChangeColor);

        monsterMesh.materials = materialsList.ToArray();
    }

    public void ChangeAccessory(CustomizationItem item, bool _animation = true)
    {
        if (_animation && equippedItems.Contains(item) && (item.itemName != "No Hat" && item.itemName != "No Glasses" && item.itemName != "Default"))
        {
            if (!(gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Lizard Pose") || gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Leo Pose") || gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Bat Pose")))
            {
                gameObject.GetComponent<Animator>().SetTrigger("Equip");
            }
        }
        for (int i = 0; i < accessoriesEmpty.transform.childCount; i++)
        {
            PlayerManager pm = PlayerManager.PLAYERMANAGER;
            if (accessoriesEmpty.transform.GetChild(i).name == item.itemName)
            {
                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(false);
            }

            switch (item.accessoryType)
            {
                case AccessoryType.Hat:
                    switch (creatureName)
                    {
                        case "Flyer":
                            if (accessoriesEmpty.transform.GetChild(i).name == item.itemName ||
                                accessoriesEmpty.transform.GetChild(i).name == pm.glasses[pm.flyerAppliedIndices[1]].itemName)
                            {
                                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(true);
                            }
                            else
                            {
                                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(false);
                            }
                            break;
                        case "Jumper":
                            if (accessoriesEmpty.transform.GetChild(i).name == item.itemName ||
                                accessoriesEmpty.transform.GetChild(i).name == pm.glasses[pm.jumperAppliedIndices[1]].itemName)
                            {
                                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(true);
                            }
                            else
                            {
                                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(false);
                            }
                            break;
                        case "Swapper":
                            if (accessoriesEmpty.transform.GetChild(i).name == item.itemName ||
                                accessoriesEmpty.transform.GetChild(i).name == pm.glasses[pm.swapperAppliedIndices[1]].itemName)
                            {
                                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(true);
                            }
                            else
                            {
                                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(false);
                            }
                            break;
                    }
                    break;
                case AccessoryType.Glasses:
                    switch (creatureName)
                    {
                        case "Flyer":
                            if (accessoriesEmpty.transform.GetChild(i).name == item.itemName ||
                                accessoriesEmpty.transform.GetChild(i).name == pm.hats[pm.flyerAppliedIndices[0]].itemName)
                            {
                                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(true);
                            }
                            else
                            {
                                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(false);
                            }
                            break;
                        case "Jumper":
                            if (accessoriesEmpty.transform.GetChild(i).name == item.itemName ||
                                accessoriesEmpty.transform.GetChild(i).name == pm.hats[pm.jumperAppliedIndices[0]].itemName)
                            {
                                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(true);
                            }
                            else
                            {
                                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(false);
                            }
                            break;
                        case "Swapper":
                            if (accessoriesEmpty.transform.GetChild(i).name == item.itemName ||
                                accessoriesEmpty.transform.GetChild(i).name == pm.hats[pm.swapperAppliedIndices[0]].itemName)
                            {
                                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(true);
                            }
                            else
                            {
                                accessoriesEmpty.transform.GetChild(i).gameObject.SetActive(false);
                            }
                            break;
                    }
                    break;
            }
        }
    }
}
