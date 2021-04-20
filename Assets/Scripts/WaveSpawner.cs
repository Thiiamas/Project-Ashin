using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

    [System.Serializable] 
    public class Wave
    {
        public GameObject[] enemiesPrefab;
        private List<Enemy> enemies = new List<Enemy>();
        public bool IsFinished { get { return enemies.Count == 0; }   }

        public void AddEnemy(Enemy newEnemy) 
        { 
            enemies.Add(newEnemy);   
        }

        public void RemoveEnemy(Enemy enemyToRemove) 
        { 
            enemies.Remove(enemyToRemove);   
        }
    }

    [SerializeField] private Wave[] waves;
    [SerializeField] Transform[] spawnPoints;
    private int currentWaveIndex = 0;
    public Wave CurrentWave { get { return waves[currentWaveIndex]; }   }


    void Start()
    {
        SpawnWave(waves[currentWaveIndex]);
    }


    void Update()
    {
        if(waves[currentWaveIndex].IsFinished)
        {
            if(currentWaveIndex + 1 >= waves.Length){
                currentWaveIndex = 0;
            } else {
                currentWaveIndex++;
            } 
            SpawnWave(waves[currentWaveIndex]);
        }
    }

    void SpawnWave(Wave wave)
    {
        List<Transform> spawnPointsCopy = new List<Transform>(spawnPoints);
        int spawnPointIndex = 0;

        foreach(GameObject enemyPrefab in wave.enemiesPrefab)
        {
            spawnPointIndex = Random.Range(0,spawnPointsCopy.Count);
            Enemy enemy = SpawnEnemy(enemyPrefab, spawnPointsCopy[spawnPointIndex].position);
            wave.AddEnemy(enemy);
            spawnPointsCopy.RemoveAt(spawnPointIndex);
        }
    }

    Enemy SpawnEnemy(GameObject enemyPrefab, Vector3 position)
    {
        return Instantiate(enemyPrefab, position, Quaternion.identity).GetComponent<Enemy>();
    }



}
