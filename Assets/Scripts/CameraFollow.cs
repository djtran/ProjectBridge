using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject playerCharacter;
    public Camera playerCamera;

    Vector3 distanceToPlayer;
    Quaternion cameraRotation;
	// Use this for initialization
	void Start () {
        distanceToPlayer = playerCharacter.transform.position - playerCamera.transform.position;
        cameraRotation = playerCamera.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        playerCamera.transform.SetPositionAndRotation(playerCharacter.transform.position - distanceToPlayer, cameraRotation);
	}
}
