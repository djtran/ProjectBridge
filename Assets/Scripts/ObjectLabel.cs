using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLabel : MonoBehaviour {

    public GameObject labelPrefab;
    public string name;

    GameObject label;
    Camera camToUse = Camera.main;

    // Use this for initialization
    void Start () {
        label = (GameObject)Instantiate(labelPrefab);
        label.GetComponent<TextMesh>().text = name;
    }
	
	// Update is called once per frame
	void Update () {
        label.transform.SetPositionAndRotation(transform.position + Vector3.up, Quaternion.Euler(30.0f, 0.0f, 0.0f));
	}
}
