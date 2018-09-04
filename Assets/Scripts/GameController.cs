﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject[] hazards;
    public Vector3 spawnValues;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;

    //public Text scoreText;
    //public Text restartText;
    //public Text gameOverText;

    private bool gameOver;
    private bool restart;
    private int score;

    int beat = BGM.beats;
    public int spawnOffset;

    int nextBeat;

    void Start()
    {
        gameOver = false;
        restart = false;
        //restartText.text = "";
        //gameOverText.text = "";
        score = 0;
        UpdateScore();
        nextBeat = spawnOffset;
    }

    void Update()
    {
        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (BGM.beats > nextBeat)
        {
            Vector3 spawnPosition = new Vector3(spawnValues.x, Random.Range(0, spawnValues.y), spawnValues.z);
            var tmp = SpawnEnemy(spawnPosition, hazards[0]);
            tmp.GetComponent<Mover>().target = new Vector3(spawnPosition.x + 20, spawnPosition.y, 0);
            nextBeat += spawnOffset;
        }
    }
    
    GameObject SpawnEnemy(Vector3 pos, GameObject type)
    {
        return Instantiate(type, pos, Quaternion.identity);
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        //scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        //gameOverText.text = "Game Over!";
        gameOver = true;
    }
}
