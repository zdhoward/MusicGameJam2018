using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover_Player : ProjectileMover {
    public override void AddToStartup()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }

    public override void Move() // disables this for parent class
    {

    }
}
