using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float speed;
    public Vector3 target;

	// Use this for initialization
	void Start () {
        Move();
    }
	
	// Update is called once per frame
	void Update () {
        Move();
	}

    public virtual void Move()
    {
        transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), target, speed * Time.deltaTime);
    }
}
