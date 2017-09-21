using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour {

	public float fpsTargetDistance;
	public float enemyLookDistance;
	public float attackDistance;
	public Transform fpsTarget;
	public Vector3 gunOffset;

	float slerpModifier = 1.5f;
	Rigidbody AIBody;
	Renderer AIRender;

	float rotate = 100f;

	// Use this for initialization
	void Start () {
		AIRender = GetComponent<Renderer>();
		AIBody = GetComponent<Rigidbody>();
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
				//I see you, but not firing yet
				//AIRender.material.color = Color.yellow;
				//show "Kill" label
			}
			else
			{
				shootPlayer();
			}
			lookAtPlayer();

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
			var gunController = GetComponent<BossGunController>();
			if(gunController!=null)
			{
				gunController.Shoot();
			}
		}
	}
}
