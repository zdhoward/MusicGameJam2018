using System.Collections;
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

    //public GameObject enemy;

    int nextBeat;

    void Start()
    {
        gameOver = false;
        restart = false;
        //restartText.text = "";
        //gameOverText.text = "";
        score = 0;
        UpdateScore();
        //StartCoroutine(SpawnWaves());
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
            SpawnEnemy(spawnPosition, hazards[0]);
            nextBeat += spawnOffset;
        }
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            for (int i = 0; i < hazardCount; i++)
            {
                GameObject hazard = hazards[Random.Range(0, hazards.Length)];
                Vector3 spawnPosition = new Vector3(spawnValues.x, Random.Range(0, spawnValues.y), spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(hazard, spawnPosition, spawnRotation);
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);

            if (gameOver)
            {
                //restartText.text = "Press 'R' for Restart";
                restart = true;
                break;
            }
        }

    }

    void SpawnEnemy(Vector3 pos, GameObject type)
    {
        Instantiate(type, pos, Quaternion.identity);
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
