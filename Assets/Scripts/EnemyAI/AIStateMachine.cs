using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : MonoBehaviour {

    public float fpsTargetDistance;
    public float enemyLookDistance;
    public float attackDistance;
    public Transform fpsTarget;
    public Vector3 gunOffset;

    float slerpModifier = 6.0f;
    Rigidbody AIBody;
    Renderer AIRender;

	// Use this for initialization 
	void Start () {
        AIRender = GetComponent<Renderer>();
        AIBody = GetComponent<Rigidbody>();
        AIRender.material.color = Color.green;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (fpsTarget == null) {
			return;
		}

        fpsTargetDistance = Vector3.Distance(fpsTarget.position, transform.position);
        if (fpsTargetDistance < enemyLookDistance)
        {
            if(fpsTargetDistance > attackDistance)
            {
                AIRender.material.color = Color.yellow;
            }
            else
            {
                AIRender.material.color = Color.red;
                shootPlayer();
            }
            lookAtPlayer();

        }
        else
        {
            AIRender.material.color = Color.green;
        }
	}

    void lookAtPlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(fpsTarget.position + gunOffset - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * slerpModifier);
    }

    void shootPlayer ()
    {
        Quaternion rotation = Quaternion.LookRotation(fpsTarget.position + gunOffset - transform.position);
        if(Quaternion.Angle(rotation,transform.rotation) <= 10.0f)
        {
            var gunController = GetComponent<GunController>();
            if(gunController!=null)
            {
                gunController.Shoot();
            }
        }
    }
}
