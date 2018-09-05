using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack_Normal : MonoBehaviour {

    public GameObject shot;

    int beat;

    Vector3 target;

    Transform shotSpawn;

    public int rateOfFire; // normal 2 = every 1/2 note

	// Use this for initialization
	void Start () {
        beat = BGM.beats;
	}
	
	// Update is called once per frame
	void Update () {
        if (beat + rateOfFire <= BGM.beats)
        {
            shotSpawn = gameObject.transform;
            target = GameObject.Find("Player").transform.position;
            Fire();
            beat = BGM.beats;
        }
	}

    void Fire ()
    {
        var obj = Instantiate(shot, shotSpawn.position + new Vector3(-1, 0, 0), shotSpawn.rotation);
        obj.GetComponent<Mover_EnemyProjectile_Wavy1>().speed = 8;
        obj.GetComponent<Mover_EnemyProjectile_Wavy1>().target = target + new Vector3(-5,0,0);
    }
}
