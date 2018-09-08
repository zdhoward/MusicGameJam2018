using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByContact : MonoBehaviour {

    public GameObject ActualObject;
    public GameObject explosion;
    public int scoreValue;
    private GameController gameController;
    public bool WeakPoint;
    

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(tag == "Projectile_Player")
        {
            Destroy(gameObject);
            return;
        }


        
        if (other.tag == "Boundary" || other.tag == "Enemy")
        {
            return;
        }

        if (explosion != null)
        {
            Instantiate(explosion, transform.position, transform.rotation);
        }

        
        if (other.tag == "Player")
        {



            //Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
            GameObject.Find("Player").GetComponent<PlayerController>().DamagePlayer();
            Destroy(ActualObject.gameObject);
        }

        if (other.tag == "Projectile_Player") // exempt enemy projectiles here if desired
        {
            Debug.Log(ActualObject.transform.tag);
            if (ActualObject.transform.tag == "Enemy")
            {
                var ea = ActualObject.GetComponent<MinionAttack>();

                if (WeakPoint)
                    ea.Health--;
                Destroy(other.gameObject);
                if (ea.Health < 0)
                {
                    Destroy(ActualObject.gameObject);

                }
            }
            else
            {
                gameController.AddScore(scoreValue);
                Destroy(ActualObject.gameObject);
            }
        }




    }
}
