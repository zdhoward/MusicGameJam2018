using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover_EnemyProjectile : MonoBehaviour {

    public int speed;

	// Use this for initialization
	void Start () {
        transform.LookAt(GameObject.Find("Player").transform.position);
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
