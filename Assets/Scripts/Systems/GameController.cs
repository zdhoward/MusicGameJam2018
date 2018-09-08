using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject[] hazards;
    public Vector3 SpawnValuesMin;
    public Vector3 SpawnValuesMax;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;

    public Text scoreText;
    //public Text restartText;
    public Text gameOverText;

    private bool gameOver;
    private bool restart;
    private int score;

    public int spawnOffset;

    int nextBeat;

    FMOD.Studio.EventInstance musicInstance;

    GameObject player;

    void Start()
    {
        gameOver = false;
        restart = false;
        //restartText.text = "";
        gameOverText.text = "";
        score = 0;
        UpdateScore();
        nextBeat = spawnOffset;

        player = GameObject.Find("Player");
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

        if (BGM.beats >= nextBeat)
        {
            Vector3 spawnPosition = new Vector3(player.transform.position.x + 20, Random.Range(SpawnValuesMin.y, SpawnValuesMax.y), SpawnValuesMax.z);
            var tmp = SpawnEnemy(spawnPosition, hazards[0]);
            tmp.GetComponent<EnemyController>().target = new Vector3(spawnPosition.x + 100, spawnPosition.y, 0);
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
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        // play game over music
        GameObject.Find("BGM").GetComponent<BGM>().SwitchToGameOverMusic();
        gameOverText.text = "Game Over!";
        gameOver = true;
    }
}
