using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MinionAttack : EnemyAttack {

    // Use this for initialization
    // Update is called once per frame
    public float HurlPower;

    void Update () {

        GetTarget();
        if(BGM.beats >= nextBeat)
        {
            HurlToPlayer();
            nextBeat += rateOfFire;
        }
        



    }

    void HurlToPlayer()
    {
        Debug.Log("Hurling");
        Vector3 dir = (target - transform.position).normalized;
        var a = transform.DOMove(transform.position + dir * 3f, 0.5f);
        //a.SetEase(Ease.InBounce);
        //GetComponent<Rigidbody2D>().AddForce(dir.normalized * HurlPower);
    }
}
