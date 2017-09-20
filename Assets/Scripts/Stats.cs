using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour {

    public GameObject healthbarPrefab;
    public float healthMax = 10.0f;
    public float recover1HPCooldown = 2.5f;

    private GameObject healthbar;
    private float health;
    
    // Use this for initialization
    void Start () {
        health = healthMax;
        healthbar = (GameObject)Instantiate(healthbarPrefab);
        StartCoroutine("recover");
	}

    private void Update()
    {

        healthbar.transform.SetPositionAndRotation(this.transform.position + Vector3.up, Quaternion.Euler(30.0f, 0.0f, 0.0f));
    }

    public void anyDamage(float damage)
    {
        Debug.Log(this.name + " HP: " + health);
        health = Mathf.Clamp(health - damage, 0, healthMax);
        updateHealthBar();

        if (health == 0.0f)
        {
            Destroy(this.gameObject, 0.0f);
        }
    }

    IEnumerator recover()
    {
        if (health < healthMax)
        {
            health = Mathf.Clamp(health + 1.0f, 0, healthMax);
        }
        updateHealthBar();

        yield return new WaitForSeconds(recover1HPCooldown);
    }

    void updateHealthBar()
    {
        if (healthbar != null)
        {
            healthbar.GetComponentInChildren<Slider>().value = health / healthMax;
        }
    }
}
