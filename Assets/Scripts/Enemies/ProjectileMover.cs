using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour {

    public float speed;
    public Vector3 target;
    public float frequency = 20;  // Rate of sine movement
    public float magnitude = 0.5f;   // Size of sine movement
    public Vector3 axis;

    void Start()
    {
        axis = transform.up;
        Move();
        AddToStartup();
    }

    void Update()
    {
        Move();
        if (transform.position == target)
            Destroy(gameObject);
    }

    public virtual void Move()
    {
        transform.position = Vector2.MoveTowards(new Vector3(transform.position.x, transform.position.y, 0f), target, speed * Time.deltaTime);
    }

    public virtual void AddToStartup ()
    {

    }
}
