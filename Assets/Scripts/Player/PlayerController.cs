﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, yMin, yMax;
}

public class PlayerController : MonoBehaviour
{
    public int maxHealth;
    public int health;

    public float speed;
    public float tilt;
    public Boundary boundary;

    public GameObject shot;
    Transform shotSpawn;
    public float fireRate;

    private float nextFire;

    public Text healthText;

    bool lookingRight = true;

    GameController gameController;

    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        shotSpawn = gameObject.transform;
    }

    void Update()
    {
        float look = Camera.main.ScreenToViewportPoint(Input.mousePosition).x;
        lookingRight = look > 0.5f ? true : false;

        if(lookingRight)
        {
            transform.localScale  = new Vector3(4, 4, 1);
        }
        else
        {
            transform.localScale = new Vector3(-4, 4, 1);
        }

        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Vector3 diff = lookingRight ? new Vector3(0.9f, 0.9f, 0) : new Vector3(-0.9f, 0.9f, 0);

            var obj = Instantiate(shot, shotSpawn.position + diff, shotSpawn.rotation);

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -10f;

            Vector3 direction = Camera.main.ScreenToWorldPoint(mousePos) - transform.position;

            obj.GetComponent<ProjectileMover>().target = direction * 100f;
            obj.GetComponent<ProjectileMover_Player>().PlayerRight = lookingRight;
        }

        UpdateHealthText();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector3(moveHorizontal, moveVertical);
        GetComponent<Rigidbody2D>().velocity = movement * speed;

        
        GetComponent<Rigidbody2D>().position = new Vector2
        (
            Mathf.Clamp(GetComponent<Rigidbody2D>().position.x, boundary.xMin, boundary.xMax),
            Mathf.Clamp(GetComponent<Rigidbody2D>().position.y, boundary.yMin, boundary.yMax)
        );
        
        //GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
    }

    public void DamagePlayer()
    {
        health--;
        if (health <= 0)
        {
            gameController.GameOver();
            UpdateHealthText();
            Destroy(gameObject);
        }
    }

    void UpdateHealthText()
    {
        healthText.text = ("Health: " + health);
    }
}