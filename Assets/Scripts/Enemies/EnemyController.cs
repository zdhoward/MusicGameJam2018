using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float speed;
    public Vector3 target;
    public GameObject targetObject;

    int nextBeat;
	// Use this for initialization
	void Start () {
        targetObject = GameObject.Find("Player");
        Move();
    }
	
	// Update is called once per frame
	void Update () {
        Move();
	}

    public virtual void Move()
    {
        target = targetObject.transform.position;
        transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), target, speed * Time.deltaTime);
    }
}
