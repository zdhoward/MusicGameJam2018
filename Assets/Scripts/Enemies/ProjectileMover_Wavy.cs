using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover_Wavy : ProjectileMover {

	public override void Move()
    {
        transform.position = Vector2.MoveTowards(new Vector3(transform.position.x, transform.position.y, 0f) + (axis * Mathf.Sin(Time.time * frequency) * magnitude), target, speed * Time.deltaTime);
    }
}
