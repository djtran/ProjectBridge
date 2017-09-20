using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public GameObject gun1;
    public GameObject gun2;
    public GameObject bulletPrefab;
    public float coolDownInSeconds = .2f;
    public float movementSpd = 5.0f;
    public float damage = 1.0f;

    private bool bAlternate;
    private float lastShot;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * movementSpd;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        if (Input.GetKey(KeyCode.Space))
        {
            if(Time.time >= lastShot + coolDownInSeconds)
            {
                Fire(gun1);
                bAlternate = !bAlternate;
                lastShot = Time.time;
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.gameObject.name.Equals("Bullet"))
        {
            movementSpd = 0.0f;
        }
    }

    void Fire (GameObject gun)
    {
        var fireDirection = gun.transform.forward;
        var bullet = (GameObject)Instantiate(bulletPrefab, gun.transform.position, gun.transform.rotation);
        if(bullet.GetComponent<BulletDamage>() == null)
        {
            bullet.AddComponent<BulletDamage>();
        }
        bullet.GetComponent<BulletDamage>().damageOnHit = damage;
        bullet.GetComponent<Rigidbody>().velocity = fireDirection * 10.0f;
        Destroy(bullet, 3.0f);

    }
}
