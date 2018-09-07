using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {


    public GameObject Player;
    public GameObject[] StaticBackgroundObjects;

	// Use this for initialization
	void Start () {
        StaticBackgroundObjects = GameObject.FindGameObjectsWithTag("StaticBackground");
        Player = GameObject.Find("Player");
        lastPos = transform.position.x;
	}

    // Update is called once per frame
    float lastPos;
	void Update () {

        float look = Camera.main.ScreenToViewportPoint(Input.mousePosition).x - 0.5f;

        Vector3 pos = transform.position;
        
      
        if (Player != null)
        {
            Vector3 target = new Vector3(Player.transform.position.x + look * 8f, pos.y, pos.z);
            transform.position = Vector3.Lerp(transform.position, target, 0.04f);
        }
            
        foreach(var o in StaticBackgroundObjects)
        {

            float diff = transform.position.x - lastPos;
            o.transform.position = new Vector3(o.transform.position.x + diff, o.transform.position.y, o.transform.position.z); 
        }
        lastPos = transform.position.x;


    }
}
