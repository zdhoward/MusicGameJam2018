﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover_PlayerProjectile : MonoBehaviour {

    public int speed;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }
}
