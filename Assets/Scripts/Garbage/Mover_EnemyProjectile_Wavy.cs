

using UnityEngine;
using System.Collections;

public class Mover_EnemyProjectile_Wavy : MonoBehaviour
{

    public float speed = -5.0f;

    public float frequency = 20.0f;  // Speed of sine movement
    public float magnitude = 0.5f;   // Size of sine movement
    private Vector3 axis;

    private Vector3 pos;

    void Start()
    {
        pos = transform.position;
        //DestroyObject(gameObject, 1.0f);
        axis = -transform.right;  // May or may not be the axis you want

    }

    void Update()
    {
        pos -= transform.up * Time.deltaTime * speed;
        transform.position = pos + axis * Mathf.Sin(Time.time * frequency) * magnitude;
    }
}

