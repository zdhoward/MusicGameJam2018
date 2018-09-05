using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover_EnemyProjectile_Wavy1 : MonoBehaviour
{

    public float speed;
    public Vector3 target;

    public float frequency = 100f;  // Rate of sine movement

    public float magnitude = 100f;   // Size of sine movement
    private Vector3 axis;

    void Start()
    {
        axis = transform.up;  // May or may not be the axis you want
        //GetComponent<Rigidbody2D>().velocity = transform.right * speed;
        transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), target, speed * Time.deltaTime);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(new Vector3(transform.position.x, transform.position.y, 0f) + (axis * Mathf.Sin(Time.time * frequency) * magnitude), target, speed * Time.deltaTime);
    }
}
