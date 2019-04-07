using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementTile : MonoBehaviour
{
    public List<GameObject> creatures;
    public int maxCreatures = 0;

    void Awake()
    {
        creatures = new List<GameObject>();
    }

    /// <summary>
    /// Setter for creature of tile
    /// </summary>
    /// <param name="_creature"></param>
    public void AddCreature(GameObject _creature)
    {
        if (creatures.Count >= maxCreatures)
        {
            Debug.Log("in");
            GameObject creature = creatures[0];

            Placer creaturePlacer = creature.GetComponent<Placer>();    //Get
            Placer otherPlacer = _creature.GetComponent<Placer>();      //Placers

            creaturePlacer.tile.creatures.Add(_creature);               //Add
            otherPlacer.tile.creatures.Add(creatures[0]);               //Creatures

            creaturePlacer.tile.RemoveCreature(creatures[0]);           //Remove
            otherPlacer.tile.RemoveCreature(_creature);                 //Creatures

            creaturePlacer.startTile = otherPlacer.tile;
            creaturePlacer.SetTile(otherPlacer.tile);                   //Set
            otherPlacer.SetTile(this);                                  //Tiles
        }
        else
        {
            
            Placer _creaturePlacer = _creature.GetComponent<Placer>();
            if (_creaturePlacer.tile != this)
            {
                if (_creaturePlacer.tile != null)
                {
                    _creaturePlacer.tile.RemoveCreature(_creature);         //Remove creature from other tile list
                }
                creatures.Add(_creature);                                   //Add creature to this tile list
               
                _creaturePlacer.SetTile(this);
            }
            
        }

        if (gameObject.tag == "Boat")
        {
            TurnCreatureToForward(_creature);
        }
        else if (gameObject.tag == "Customization") 
        {
            TurnCreatureToForward(_creature);
            CustomizationMenuManager.CUSTOMIZATIONMENUMANAGER.selectedMonster = 
                _creature;
            CustomizationMenuManager.CUSTOMIZATIONMENUMANAGER.Open();
            BeachManager.BEACHMANAGER.Order(4);
            BeachManager.BEACHMANAGER.HideBackButton(false);
        }
        
    }

    public void TurnCreatureToForward(GameObject creature)
    {
        creature.transform.position = new Vector3(transform.position.x, creature.transform.position.y, transform.position.z);

        creature.transform.LookAt(
            new Vector3(transform.forward.x * -1 + creature.transform.position.x,
                        creature.transform.position.y,
                        transform.forward.z * -1 + creature.transform.position.z));
    }

    public void RemoveCreature(GameObject _creature)
    {
        creatures.Remove(_creature);
    }

    public string GetName()
    {
        return gameObject.name;
    }
}
