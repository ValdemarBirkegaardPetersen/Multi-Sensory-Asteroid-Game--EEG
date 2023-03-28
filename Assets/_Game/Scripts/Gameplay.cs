using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
    public int numAsteroids = 0;
    public int startingNumAsteroids = 4;
    // public GameObject bullet;
    public GameObject asteroid;
    // public GameObject asteroidContainer;

    private float asteroidSpawnTime = 5.0f;
    private float lastAsteroidSpawnTime = 0.0f;

    public TMP_Text scoreText;
    public float score = 0;
    
    private void Start()
    {
        Cursor.visible = false;
        
        asteroid.SetActive(false);
        CreateAsteroids(startingNumAsteroids);
    }


    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.8f);

        // updateing score
        scoreText.text = score.ToString();

        if (Time.time > lastAsteroidSpawnTime + asteroidSpawnTime)
        {
            lastAsteroidSpawnTime = Time.time;

            // Implementation for spawn over time
            if (Time.time > 30.0f)
            {
                asteroidSpawnTime = 2.5f;
            }
            Debug.Log(asteroidSpawnTime);
            CreateAsteroids(1);
        }

        // Implementation for wave spawn
        /*respawning asteroids after killed them all
        if (numAsteroids < 1)
        {
            CreateAsteroids(startingNumAsteroids);
        }
        */
    }

    private void CreateAsteroids(int asteroidsNum)
    {
        numAsteroids = asteroidsNum;

        for (int i = 1; i <= asteroidsNum; i++) {
            GameObject asteroidsClone = Instantiate(asteroid, new Vector2(Random.Range(-10, 10), Random.Range(-10, 10)), transform.rotation);
            // asteroidsClone.transform.parent = asteroidContainer.transform;
            asteroidsClone.SetActive(true);
        }
    }

     public void RocketFail()
    {
        print("ROCKET IS DEAD");

    }
    
}
