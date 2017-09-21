using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionAbilities : MonoBehaviour {

    public static List<CompanionAbilities> _instances;

    public Vector3 gunOffset;
    public float attackDistance = 25.0f;

    private float slerpModifier = 5.0f;
    public GameObject currentTarget;
    public ArrayList targetableEnemies;
    private SphereCollider detectRadius;
    public int state = 0;
    public string targetName;
    private DistanceComparer distanceComparer;

    //Navigation
    public Transform startPoint;
    public Transform endPoint;
    private NavMeshAgent agent;

	// Use this for initialization
	void Start () {
        detectRadius = GetComponentInChildren<SphereCollider>();
        targetableEnemies = new ArrayList();
        distanceComparer = new DistanceComparer();
        distanceComparer.originator = this.gameObject;
        agent = GetComponent<NavMeshAgent>();
        
        if(_instances == null)
        {
            _instances = new List<CompanionAbilities>();
        }
        _instances.Add(this);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if(currentTarget == null)
        {
            state = 0;
        }
        switch (state)
        {
            case 1:
                advance();
                break;
            case 2:
                retreat();
                break;
            case 3:
                focusTarget();
                break;
            default:
                defendPosition();
                break;
        }
	}

    public void receiveCommand(string command)
    {
		Debug.Log ("Command Received!: " + command);
        string[] commandList = command.Split(' ');
        targetName = null;

        switch (commandList[1])
        {
            case "advance":
                state = 1;
                break;
            case "takeCover":
                state = 2;
                break;
            case "fire":
                state = 3;
                targetName = commandList[2];
                foreach (GameObject target in targetableEnemies)
                {
                    if (target.GetComponent<ObjectLabel>().name == targetName || target.GetComponent<ObjectLabel>().name.Equals(targetName))
                    {
                        currentTarget = target;
                    }
                }
                break;
            default:
                state = 0;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<ObjectLabel>() != null)
        {
            if(!targetableEnemies.Contains(other.gameObject))
            {
                targetableEnemies.Add(other.gameObject);
            }
        }
        targetableEnemies.Sort(distanceComparer);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent<ObjectLabel>() != null)
        {
            if (targetableEnemies.Contains(other.gameObject))
            {
                targetableEnemies.Remove(other.gameObject);
            }
        }
    }

    void defendPosition()
    {
        if(targetableEnemies.Count != 0)
        {
            var i = 0;
            GameObject target = (GameObject) targetableEnemies[i];

            while (i < targetableEnemies.Capacity)
            {
                target = (GameObject)targetableEnemies[i];
                if(target == null)
                {
                    i++;
                    continue;
                }
                if (target.GetComponent<PlayerController>() != null)
                {
                    i++;
                    continue;
                }
                else if (target.GetComponent<CompanionAbilities>() != null)
                {
                    i++;
                    continue;
                }
                break;
            }
            currentTarget = target;
            if(Vector3.Distance(this.transform.position, target.transform.position) <= attackDistance)
            {
                attackTarget();
            }
        }
    }

    void advance()
    {
		Debug.Log ("Advancing");
        if (currentTarget != null)
        {
            if (!lineOfSight(currentTarget.transform) && attackDistance < Vector3.Distance(this.transform.position, currentTarget.transform.position))
            {
                agent.SetDestination(currentTarget.transform.position);
                agent.isStopped = false;
                targetName = currentTarget.GetComponent<ObjectLabel>().name;
            }
            else
            {
                state = 3;
                foreach (GameObject target in targetableEnemies)
                {
                    if (target.GetComponent<ObjectLabel>().name == targetName || target.GetComponent<ObjectLabel>().name.Equals(targetName))
                    {
                        currentTarget = target;
                    }
                }
            }
        }
        else
        {
            agent.SetDestination(endPoint.position);
            agent.isStopped = false;
        }

    }

    void retreat()
    {
        agent.SetDestination(startPoint.position);
        agent.isStopped = false;

        StartCoroutine("Stop");
    }

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(10);

        agent.isStopped = true;
        state = 0;
    }

    void focusTarget()
    {

        if(currentTarget == null)
        {
            targetableEnemies.Clear();
            state = 0;
            return;
        }
        attackTarget();
    }

    void attackTarget()
    {
        if (!lineOfSight(currentTarget.transform) && attackDistance < Vector3.Distance(this.transform.position, currentTarget.transform.position))
        {
            agent.SetDestination(currentTarget.transform.position);
            agent.isStopped = false;
        }
        else
        {
            lookAtPlayer(currentTarget.transform);
            shootPlayer(currentTarget.transform);
        }
    }

    void lookAtPlayer(Transform fpsTarget)
    {
        Quaternion rotation = Quaternion.LookRotation(fpsTarget.position + gunOffset - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * slerpModifier);
    }

    void shootPlayer(Transform fpsTarget)
    {
        Quaternion rotation = Quaternion.LookRotation(fpsTarget.position + gunOffset - transform.position);
        if (Quaternion.Angle(rotation, transform.rotation) <= 10.0f && Vector3.Distance(this.transform.position, fpsTarget.transform.position) < attackDistance)
        {
            var gunController = GetComponent<GunController>();
            if (gunController != null)
            {
                gunController.Shoot();
            }
        }
    }

    bool lineOfSight(Transform fpsTarget)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, fpsTarget.position - transform.position, out hit);
        if(hit.collider.gameObject.GetComponent<ObjectLabel>() != null)
        {
            return true;
        }
        return false;
    }
}