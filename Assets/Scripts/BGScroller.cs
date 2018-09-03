using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroller : MonoBehaviour
{
    //public float scrollSpeed;
    //public float tileSizeX;

    //private Vector3 startPosition;

    void Start()
    {
        //startPosition = transform.position;
    }

    void Update2()
    {
        /* To Scroll an image of set size
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeX);
        transform.position = startPosition + Vector3.left * newPosition;
        */
    }

    public float rotateSpeed = 1.1f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
    }
}
