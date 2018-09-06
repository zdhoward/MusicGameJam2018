using UnityEngine;
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

    GameController gameController;

    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        shotSpawn = gameObject.transform;
        //shotSpawn.position = gameObject.transform.position + new Vector3(3f,0f,0f);
        //shotSpawn.rotation = gameObject.transform.rotation;
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            var obj = Instantiate(shot, shotSpawn.position + new Vector3(1,0,0), shotSpawn.rotation);
            //obj.GetComponent<ProjectileMover>().speed = 8;
            obj.GetComponent<ProjectileMover>().target = GameObject.Find("Player").transform.position + new Vector3(100f,0f,0f);
            //GetComponent<AudioSource>().Play();
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
            //game over
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