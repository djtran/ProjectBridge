using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour {

    public GameObject healthbar;
    public float healthMax = 10.0f;
    public float recover1HPCooldown = 2.5f;
    private float health;
    // Use this for initialization
    void Start () {
        health = healthMax;
        StartCoroutine("recover");
	}

    public void anyDamage(float damage)
    {
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
            healthbar.GetComponent<Slider>().value = health / healthMax;
        }
    }
}
