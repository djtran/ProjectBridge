﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour {

    public GameObject smokePrefab;
    public GameObject explosionPrefab;
    public GameObject healthbarPrefab;
    public float healthMax = 10.0f;
    public float recover1HPCooldown = 2.5f;
    public bool dead = false;
    public float upDistance = 1.0f;

    private GameObject healthbar;
    private float health;
    private GameObject smoke;
    private GameObject explode;
    
    // Use this for initialization
    void Start () {
        health = healthMax;
        healthbar = (GameObject)Instantiate(healthbarPrefab);
        StartCoroutine("recover");
	}

    private void Update()
    {

        healthbar.transform.SetPositionAndRotation(this.transform.position + Vector3.up*upDistance, Quaternion.Euler(30.0f, 0.0f, 0.0f));
    }

    public void anyDamage(float damage)
    {
        Debug.Log(this.name + " HP: " + health);
        health = Mathf.Clamp(health - damage, 0, healthMax);
        updateHealthBar();

        if (health == 0.0f)
        {
            GetComponent<ObjectLabel>().setDead();
            if(!dead)
            {
                explode = Instantiate(explosionPrefab, transform);
                smoke = Instantiate(smokePrefab, transform);
            }
            dead = true;


            if (GetComponent<AIStateMachine>())
            {
                GetComponent<AIStateMachine>().dead = true;
            }
            else if (GetComponent<PlayerController>())
            {
                GetComponent<PlayerController>().dead = true;
            }
            else if (GetComponent<CompanionAbilities>())
            {
                GetComponent<CompanionAbilities>().dead = true;
            }
            else if (GetComponent<BossAI>())
            {
                GetComponent<BossAI>().dead = true;
            }
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
