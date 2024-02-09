using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSpawner : MonoBehaviour
{
    public EnemyFactory m_Factory;
    private IEnemy m_Red;
    private IEnemy m_Blue;
    private IEnemy m_Green;

    //spawn time
    public float spawnTimer; 
    //get total spawncound from game manager **  and use to to check if to keep spawning 


    private GameManagerScript.DifficultyState currentDifficulty;


    public IEnumerator Timer()
    {
        while (GameManagerScript.S.EnemySpawnTotal > 0)
        {
            yield return new WaitForSeconds(spawnTimer);
            if (GameManagerScript.S.EnemySpawnTotal > 0)
            {
                //if we can spawn one more spawn a boss.
                SpawnEnemy();
            }
           }
    }

    public void Start()
    {

        // Subscribe to the OnDifficultyChange event
        GameManagerScript.OnDifficultyChange += HandleDifficultyChange;
        StartCoroutine(Timer());
    }

    public void Update()
    {
      
    }

    public void SpawnEnemy()
    {
  
        m_Red = m_Factory.GetEnemy(EnemyType.Red,currentDifficulty);
        // m_Blue = m_Factory.GetEnemy(EnemyType.Green);
        // m_Green = m_Factory.GetEnemy(EnemyType.Blue);

    }


    //update the state of the game and ahve enemies spawn faster
    private void HandleDifficultyChange(GameManagerScript.DifficultyState newState)
    {
        float twentyPercent = 0.2f;
        currentDifficulty = newState;
            switch (newState)
            {

                case GameManagerScript.DifficultyState.Easy:
                    {
                        //spawn them faster
                        spawnTimer -= spawnTimer * twentyPercent;
                    Debug.Log("Easy spawner update");
                    }
                    break;
                case GameManagerScript.DifficultyState.Medium:
                    {
                        //how many enemies of what type
                        spawnTimer -= spawnTimer * twentyPercent;
                    Debug.Log("Med spawner update");
                }
                    break;
                case GameManagerScript.DifficultyState.Hard:
                    {
                        //how many enemies of what type
                        spawnTimer -= spawnTimer * twentyPercent;
                    Debug.Log("Hard spawner update");
                }
                    break;
            }
    }


    }


    

