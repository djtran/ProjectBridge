using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLabel : MonoBehaviour {

    public GameObject labelPrefab;
    public string name;
	public float upDistance = 1.0f;

    GameObject label;

    // Use this for initialization
    void Start () {
        label = (GameObject)Instantiate(labelPrefab);
        label.GetComponent<TextMesh>().text = name;
    }
	
	// Update is called once per frame
	void Update () {
		label.transform.SetPositionAndRotation(transform.position + Vector3.up * upDistance, Quaternion.Euler(30.0f, 0.0f, 0.0f));
	}

    public void setDead()
    {
        label.GetComponent<TextMesh>().text = name + " (Dead)";
    }
}
