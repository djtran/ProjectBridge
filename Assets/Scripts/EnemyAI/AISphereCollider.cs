using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISphereCollider : MonoBehaviour
{
	void Start() {

	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			transform.parent.gameObject.GetComponent<AIStateMachine>().fpsTarget = other.transform;
			Debug.Log ("Entered");
		}
			
	}
}

