using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : EnemyAttack {

	// Use this for initialization
	void Start () {
		
	}
    int nextBeat = 0;
    // Update is called once per frame
    void Update () {

        if(BGM.beats > nextBeat)
        {
            HurlToPlayer();
        }



    }

    void HurlToPlayer()
    {
        Vector3 dir = target - transform.position;
        GetComponent<Rigidbody>().AddExplosionForce(100f, transform.position - dir, 10f);
    }
}
