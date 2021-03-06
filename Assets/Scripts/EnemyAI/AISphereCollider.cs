﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISphereCollider : MonoBehaviour
{
	void Start() {

	}

	void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player")
        {
            if (transform.parent.gameObject.GetComponent<AIStateMachine>() != null)
            {
                transform.parent.gameObject.GetComponent<AIStateMachine>().fpsTarget = other.transform;
            }
            else
            {
                transform.parent.gameObject.GetComponent<BossAI>().fpsTarget = other.transform;
            }
        }
			
	}
}

