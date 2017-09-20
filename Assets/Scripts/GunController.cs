using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    public GameObject gun1;
    public GameObject gun2;
    public GameObject bulletPrefab;
    public float coolDownInSeconds = .2f;
    public float damage = 1.0f;

    private bool bAlternate;
    private float lastShot;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Shoot()
    {
        if (Time.time >= lastShot + coolDownInSeconds)
        {
            if (gun1 != null && gun2 != null)
            {
                if (bAlternate)
                {
                    Fire(gun1);
                }
                else
                {
                    Fire(gun2);
                }
            }
            else
            {
                if(gun1 != null)
                {
                    Fire(gun1);
                }
                else
                {
                    Fire(gun2);
                }
            }
            bAlternate = !bAlternate;
            lastShot = Time.time;
        }
    }

    private void Fire(GameObject gun)
    {
        var fireDirection = gun.transform.forward;
        var bullet = (GameObject)Instantiate(bulletPrefab, gun.transform.position, gun.transform.rotation);
        if (bullet.GetComponent<BulletDamage>() == null)
        {
            bullet.AddComponent<BulletDamage>();
        }
        bullet.GetComponent<BulletDamage>().damageOnHit = damage;
        bullet.GetComponent<Rigidbody>().velocity = fireDirection * 10.0f;
        Destroy(bullet, 3.0f);

    }
}
