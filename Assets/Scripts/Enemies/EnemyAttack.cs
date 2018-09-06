using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

    public GameObject shot;
    public Vector3 target;
    public Transform shotSpawn;
    public int rateOfFire;
    public int beatOffset;

    int nextBeat;

	void Start () {
        nextBeat = BGM.beats + beatOffset;
    }

    void Update()
    {
        if (BGM.beats >= nextBeat)
        {
            shotSpawn = gameObject.transform;
            Fire();
            nextBeat += rateOfFire;
        }
    }

    public virtual void Fire()
    {
        GetTarget();
        var obj = Instantiate(shot, shotSpawn.position + new Vector3(-1, 0, 0), shotSpawn.rotation);
        obj.GetComponent<ProjectileMover>().speed = rateOfFire;
        obj.GetComponent<ProjectileMover>().target = target;
    }

    public virtual void GetTarget()
    {
        if (GameObject.Find("Player") != null)
        {
            target = GameObject.Find("Player").transform.position;// + new Vector3(-5, 0, 0);
        } else
        {
            Debug.Log(target);
        }
    }
}
