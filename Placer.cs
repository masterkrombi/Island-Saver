using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Placer : MonoBehaviour {
    
    public PlacementTile startTile;
    public LayerMask mask;
    RaycastHit hit;
    Vector3 originalPosition;
    public PlacementTile tile;
    public AudioClip monsterClip;
    [Header("Min: -12   Max: 12")]
    public Vector2 spawnRangeX;
    [Header("Min: 15   Max: 20")]
    public Vector2 spawnRangeZ;

    [HideInInspector]
    public bool placing;

    void Start()
    {
        if (startTile != null)
        {
            startTile.AddCreature(gameObject);
        }
        else
        {
            startTile = GameObject.FindWithTag("Ground").GetComponent<PlacementTile>();
            startTile.AddCreature(gameObject);
        }
    }

    void OnMouseDown()
    {
        if (TutorialManager.inDialogue) return;

        if (!CustomizationMenuManager.customizing && BeachManager.defaultView && !BeachManager.BEACHMANAGER.departing) {
            originalPosition = transform.position;
            TapEffect.interractingWithCreature = true;
            placing = true;   
            gameObject.GetComponent<MonsterFarmBehavior>().characterAnimator.SetBool("Walking", false);

            // Show pedestal indications
            BeachManager.BEACHMANAGER.ShowPedestalIndications(true);
        }
        GetComponent<CapsuleCollider>().enabled = false;
    }

    void OnMouseDrag()
    {
        if (placing)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
            Vector3 objPosition = transform.position;
            Vector3 direction = Camera.main.ScreenToWorldPoint(mousePosition) - Camera.main.transform.position;
           
            BoxCollider box = GetComponent<BoxCollider>();

            if (Physics.BoxCast(Camera.main.transform.position, box.size, direction, out hit, box.gameObject.transform.rotation, Mathf.Infinity, mask))// out hit, 100, mask))
            {
                objPosition = Camera.main.transform.position + direction.normalized * hit.distance;
            }

            transform.position = objPosition;
        }
    }

    void OnMouseUp()
    {
        if (placing)
        {
            PlacementTile newTile = null;
            if (hit.collider != null)
            {
                if (hit.collider.tag != "NotPlacement")
                {
                    
                    newTile = hit.collider.gameObject.GetComponent<PlacementTile>();
                }
            }
            /* Possible use for future, but causes lots of issues when placing on environment objects
            if (newTile == null)
            {
                if (Physics.Raycast(new Ray(transform.position, -transform.up), out hit))
                {
                    if (hit.collider.tag != "NotPlacement")
                    {
                        newTile = hit.collider.gameObject.GetComponent<PlacementTile>();
                    }
                }
            }
            */

            if (newTile == null)
            {
                transform.position = originalPosition;
            }
            else if (newTile != null)
            {
                newTile.AddCreature(gameObject);
                if(newTile.tag == "Boat")
                {
                    gameObject.GetComponent<MonsterFarmBehavior>().characterAnimator.SetTrigger("Ready");
                }

            }
            placing = false;

            // Hide pedestal indications
            BeachManager.BEACHMANAGER.ShowPedestalIndications(false);
        }
        GetComponent<CapsuleCollider>().enabled = true;
        TapEffect.interractingWithCreature = false;
    }

    public void SetTile(PlacementTile _tile)
    {
        tile = _tile;
        if (tile == startTile)
        {
            transform.position = tile.gameObject.transform.position + 
                                 Vector3.right * Random.Range(spawnRangeX.x, spawnRangeX.y) + //-12 to 12
                                 Vector3.up * 2f + 
                                 Vector3.back * Random.Range(spawnRangeZ.x, spawnRangeZ.y); //15 to 20\
            startTile = null;
        }
        else
        {
            if (tile.tag != "Ground")
            {
                transform.position = tile.gameObject.transform.position + Vector3.up * 2f;
            }
           
        }

        if (tile.tag == "Boat")
        {
            BeachAudioManager.PlaySound(BeachSoundType.Monster, monsterClip);
        }
    }
    

    public void SetOriginal()
    {
        SetTile(startTile);
        startTile.AddCreature(gameObject);
    }
}