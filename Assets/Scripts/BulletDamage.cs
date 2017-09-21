using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour {

    public float damageOnHit = 1.0f;
    public GameObject explosionPrefab;
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        var opponentStats = collision.collider.gameObject.GetComponent<Stats>();
        if (opponentStats != null)
        {
            opponentStats.anyDamage(damageOnHit);
        }
        Destroy(this.gameObject);
    }


}
