using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFarmBehavior : MonoBehaviour
{

    public float moveSpeed;
    public float rotationSpeed;
    public float walkTime;
    float currentWalkTime = 0;
    public float waitTime;
    float currentWaitTime = 0;
    public float walkDelay;

    Placer placer;
    GameObject tile;
    bool placing;
    public Vector3 targetPosition;
    public Vector3[] path;
    bool endOfPath = true;
    bool onBoat;

    float pickNext = 0;
    public Vector2 pickDelayRange = new Vector2(10f, 30f);

    public Animator characterAnimator;
    SkinnedMeshRenderer _mesh;
    BeachManager _bm;
    Rigidbody _rb;

    void Start()
    {
        _bm = BeachManager.BEACHMANAGER;
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        _rb = GetComponent<Rigidbody>();

        if (characterAnimator != null)
            characterAnimator.SetBool("InBeach", true);
        placer = GetComponent<Placer>();
    }

    public void MakeInvisible() 
    {
        if (_mesh.enabled) {
            _mesh.enabled = false;
            GetComponent<Creature>().accessoriesEmpty.SetActive(false);
        } else {
            _mesh.enabled = true;
            GetComponent<Creature>().accessoriesEmpty.SetActive(true);
        }
    }

    void OnMouseUp()
    {
        endOfPath = true;
        StopAllCoroutines();
    }

    void Update()
    {
        // Update placing bool && Toggle placing animation on change
        if(placing != placer.placing)
        {
            if(placing == false) //placer.placing = true
            {
                
                characterAnimator.SetBool("Placing", true);
            }
            else //placer.placing = false
            {
                StopAllCoroutines();
                characterAnimator.SetBool("Placing", false);
                walkDelay = 4f;
            }
            //Update placing bool to match so animation doesn't place twice
            placing = placer.placing;
        }


        if (walkDelay >= 0)
        {
            walkDelay -= Time.deltaTime;
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            characterAnimator.SetBool("Walking", false);
            return;
        }


        if (currentWalkTime < walkTime)
        {
            Walk();
        }
        else
        {
            Wait();
        }

        

        if (!characterAnimator.GetBool("Walking"))
        {
            _rb.velocity = Vector3.zero;
        }

        _rb.angularVelocity = Vector3.zero;
        if (placer.tile.tag == "Boat") {
            onBoat = true;
        }
        else
        {
            onBoat = false;
        }
    }

    void Walk()
    {
        // Behavior if not being placed or customized
        if (!placing && !onBoat && !CustomizationMenuManager.customizing && _bm.camOrder == 0)
        {
            if (pickNext < Time.time || endOfPath)
            {
                PickTarget();
                if (validTarget(targetPosition))
                {
                    PathRequestManager.RequestPath(transform.position, targetPosition, OnPathFound);
                    pickNext = Random.Range(pickDelayRange.x, pickDelayRange.y) + Time.time;
                }
                
                endOfPath = false;
            }
        }

        currentWalkTime += Time.deltaTime;
        currentWaitTime = 0;
    }

    void Wait()
    {
        if (currentWaitTime < waitTime)
        {
            currentWaitTime += Time.deltaTime;
            currentWalkTime = 0;
        }

        characterAnimator.SetBool("Walking", false);
        StopAllCoroutines();
    }

    //Pick target for pathfinding
    void PickTarget()
    {
        int index = Random.Range(0, GridSystem.WALKABLENODES.Count);
        targetPosition = GridSystem.WALKABLENODES[index].worldPos;
    }

    bool validTarget(Vector3 target)
    {
        if (GetComponent<Creature>().creatureName == "Flyer")
        {
            if (target.x<=-8)
            {
                return true;
            }
        }
        else if(GetComponent<Creature>().creatureName == "Jumper")
        {
            if (target.x >= -8 && target.x <= 0)
            {
                return true;
            }
        }
        else if (GetComponent<Creature>().creatureName == "Swapper")
        {
            if (target.x >= 0)
            {
                return true;
            }
        }
        return false;
    } 

    //Callback Action for pathfinding request manager
    void OnPathFound(Vector3[] newPath, bool pathSuccesful)
    {
        if (pathSuccesful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    public Vector3 target;
    public Vector3 currentWaypoint;
    //Traverse path
    IEnumerator FollowPath()
    {
        if (path.Length > 0)
        {
            if (characterAnimator != null)
                characterAnimator.SetBool("Walking", true);
            currentWaypoint = path[0];

            int targetIndex = 0;
            while (!endOfPath)
            {
                if (transform.position.x == currentWaypoint.x && transform.position.z == currentWaypoint.z && !CustomizationMenuManager.customizing && !onBoat && _bm.camOrder == 0)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        if (characterAnimator != null)
                            characterAnimator.SetBool("Walking", false);
                        endOfPath = true;
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }
                else if (CustomizationMenuManager.customizing || onBoat ||_bm.camOrder != 0)
                {
                    if (characterAnimator != null)
                        characterAnimator.SetBool("Walking", false);
                }


                if (!CustomizationMenuManager.customizing && !onBoat && !placing && _bm.camOrder == 0) 
                {
                    characterAnimator.SetBool("Walking", true);
                    target = new Vector3(currentWaypoint.x, transform.position.y, currentWaypoint.z);
                    
                    if ((transform.position - target).magnitude < 0.5f)
                    {
                        transform.position = target;
                        _rb.velocity = Vector3.zero;
                    }
                    else
                    {
                        _rb.velocity = (target - transform.position).normalized * moveSpeed;
                    }
                    Rotate();
                }

                yield return null;
            }
        }
        yield return null;
    }

    void Rotate()
    {
        float newY = transform.localEulerAngles.y;

        Vector3 fromCreature;


        //Determine angle
        fromCreature = target - transform.position;
        float dotProduct = Vector3.Dot(fromCreature, transform.forward);                                  //   u  *  v
        float divisor = Mathf.Abs(fromCreature.magnitude) * Mathf.Abs(transform.forward.magnitude);       // -----------
        float angle = Mathf.Acos(dotProduct / divisor);                                                   // ||u||*||v||
        if (float.IsNaN(angle))
        {
            angle = 0;
        }


        angle *= 180 / Mathf.PI;

        float offset = Time.deltaTime * rotationSpeed;
        if (angle < offset)
        {
            offset = angle;
        }

        if (Vector3.Cross(transform.forward, fromCreature).y < 0)
        {
            newY -= offset;
        }
        else
        {
            newY += offset;
        }

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, newY, transform.localEulerAngles.z);
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "Monster")
        {
            endOfPath = true;
            StopAllCoroutines();
            walkDelay = 2f;
        }
        
    }

    float collidingTime = 0;
    void OnCollisionStay(Collision collision)
    {
        /*
        collidingTime += Time.deltaTime;
        if (collidingTime > 1f && collision.gameObject.tag == "Monster")
        {
            
            collidingTime = 0;
            endOfPath = true;
            StopAllCoroutines();
        }
        */
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Monster")
        {
            collidingTime = 0;
        }
    }
}
