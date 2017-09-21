using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionAbilities : MonoBehaviour {

    public static List<CompanionAbilities> _instances;

    public Vector3 gunOffset;
    public float attackDistance = 25.0f;

    private float slerpModifier = 5.0f;
    private Transform currentTarget;
    private ArrayList targetableEnemies;
    private SphereCollider detectRadius;
    private int state = 0;
    private string targetName;
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
        
        if(_instances == null)
        {
            _instances = new List<CompanionAbilities>();
        }
        _instances.Add(this);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        switch (state)
        {
            case 1:
                advance();
                break;
            case 2:
                retreat();
                break;
            case 3:
                focusTarget(targetName);
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

        switch (commandList[0])
        {
            case "advance":
                state = 1;
                break;
            case "takeCover":
                state = 2;
                break;
            case "fire":
                state = 3;
                targetName = commandList[1];
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
                if (target.GetComponent<PlayerController>() == null)
                {
                    break;
                }
                else
                {
                    i++;
                }
            }
            currentTarget = target.transform;
            attackTarget();
        }
    }

    void advance()
    {
		Debug.Log ("Advancing");
        if (currentTarget != null)
        {
            if (attackDistance > Vector3.Distance(this.transform.position, currentTarget.position))
            {
                agent.SetDestination(currentTarget.position);
            }
            else
            {
                agent.Stop();
                state = 3;
            }
        }
        else
        {
            agent.SetDestination(endPoint.transform.position);
        }

    }

    void retreat()
    {
        if (currentTarget == null)
        {
            agent.SetDestination(startPoint.transform.position);
        }
        else
        {
            agent.SetDestination((transform.position-currentTarget.position).normalized*15.0f);
        }
    }

    void focusTarget(string targetName)
    {

        if(currentTarget == null)
        {
            state = 0;
            return;
        }

        foreach (GameObject target in targetableEnemies)
        {
            if(target.GetComponent<ObjectLabel>().name == targetName || target.GetComponent<ObjectLabel>().name.Equals(targetName))
            {
                currentTarget = target.transform;
            }
        }

        if (attackDistance > Vector3.Distance(this.transform.position, currentTarget.position))
        {
            agent.SetDestination(currentTarget.position);
        }
        else
        {
            attackTarget();
        }
    }

    void attackTarget()
    {
        lookAtPlayer(currentTarget);
        shootPlayer(currentTarget);
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
}