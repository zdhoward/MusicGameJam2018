using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {

    public float speed;
    public Vector3 target;

    void Start()
    {
        //GetComponent<Rigidbody2D>().velocity = transform.right * speed;
        transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), target, speed * Time.deltaTime);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), target, speed * Time.deltaTime);
    }
}
