using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack_Normal : MonoBehaviour {

    public GameObject shot;

    int beat;

    Transform shotSpawn;

    public int rateOfFire; // normal 2 = every 1/2 note

	// Use this for initialization
	void Start () {
        beat = BGM.beats;
        shotSpawn = gameObject.transform;
        //StartCoroutine(Fire());
	}
	
	// Update is called once per frame
	void Update () {
        if (beat + rateOfFire <= BGM.beats)
        {
            Fire();
            beat = BGM.beats;
        }
	}

    void Fire ()
    {
        var obj = Instantiate(shot, shotSpawn.position + new Vector3(-1, 0, 0), shotSpawn.rotation);
        obj.GetComponent<Mover>().speed = -20;
    }
}
