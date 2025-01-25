using System;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance { get; private set; }

    [SerializeField] private Enemy enemyPrefab;
    [SerializeField, Min(0)] private float spawnInterval;
    
    [SerializeField] private Transform[] spawnPoints;
    
    private CountdownTimer _spawnTimer;
    
    private void Awake()
    {
        // Initialize the singleton
        Instance = this;
        
        // Initialize the spawn timer
        _spawnTimer = new CountdownTimer(spawnInterval);
        
        _spawnTimer.OnTimerEnd += () =>
        {
            // Get a random spawn point index
            var spawnPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            
            // Spawn the enemy at the spawn point
            SpawnEnemy(spawnPointIndex);
            
            // Reset the spawn timer
            _spawnTimer.SetMaxTimeAndReset(spawnInterval);
        };
        
        _spawnTimer.Start();
    }

    private void Update()
    {
        // Update the spawn timer
        _spawnTimer.SetMaxTime(spawnInterval);
        _spawnTimer.Update(Time.deltaTime);
    }

    public void SpawnEnemy(int spawnPointIndex)
    {
        // Get a random spawn point
        var spawnPoint = spawnPoints[spawnPointIndex % spawnPoints.Length];
        
        // Instantiate the enemy prefab at the spawn point
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
    
    
}