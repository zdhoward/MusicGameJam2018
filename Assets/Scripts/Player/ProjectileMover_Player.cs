using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover_Player : ProjectileMover {

    public bool PlayerRight;

    public override void AddToStartup()
    {
        //GetComponent<Rigidbody2D>().velocity = transform.right * speed;
        float angle = 0;
        if (PlayerRight)
        {
             angle = Vector2.Angle((target - transform.position), Vector3.up);
        }
        else
        {
            angle = Vector2.Angle((target - transform.position), Vector3.down);
        }
        
        Debug.Log(angle);
        transform.Rotate(Vector3.forward, -angle+90f);
        
    }

    public override void Move() // disables this for parent class
    {

        
        transform.position = Vector2.MoveTowards(new Vector3(transform.position.x, transform.position.y, 0f), target, speed * Time.deltaTime);
    }
}
