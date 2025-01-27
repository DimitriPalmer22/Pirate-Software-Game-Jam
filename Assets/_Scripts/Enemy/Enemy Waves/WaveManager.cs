using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveManager : MonoBehaviour, IDebugged
{
    #region Constants

    private const float MIN_RANDOM_TIME_BETWEEN_SPAWNS = 1f;
    private const float MAX_RANDOM_TIME_BETWEEN_SPAWNS = 5f;

    #endregion

    public static WaveManager Instance { get; private set; }

    #region Serialized Fields

    [SerializeField, Min(0)] private float timeBetweenWaves = 5;

    [SerializeField] private int[] upgradeWaves = { 2, 4 };
    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    #endregion

    #region Private Fields

    // Vars to control the spawning in the current wave
    private EnemyWave _currentWave;
    private int _currentWaveBatchIndex;
    private CountdownTimer _batchSpawnTimer;
    private readonly HashSet<Enemy> _enemiesInWave = new();
    private int _currentWaveIndex;
    private bool _isBetweenWaves;

    #endregion

    #region Getters

    public IReadOnlyList<Transform> SpawnPoints => spawnPoints;

    public int CurrentWaveIndex => _currentWaveIndex;

    public float TimeBetweenWavesRemaining { get; private set; }

    private bool IsLastBatch => _currentWaveBatchIndex >= _currentWave.EnemyBatchInfos.Length;

    public bool HasStartedGame { get; private set; }
    public bool IsWaitingForNextWave { get; private set; }

    #endregion

    public Action onWaveStart;
    public Action onWaveComplete;

    private void Awake()
    {
        // Initialize the singleton
        Instance = this;

        // Initialize the spawn timer 
        _batchSpawnTimer = new CountdownTimer(1000f);
        _batchSpawnTimer.OnTimerEnd += SpawnNextBatch;
        _batchSpawnTimer.Stop();

        // Add this to the debug manager
        DebugManager.Instance.AddDebuggedObject(this);
    }

    private void Start()
    {
        onWaveComplete += SpawnNextWaveOnWaveComplete;
        onWaveComplete += UpgradePowerOnWaveComplete;
    }

    private void SpawnNextWaveOnWaveComplete()
    {
        // After every 5 waves, wait for the player to upgrade
        if (_currentWaveIndex % 5 == 0)
        {
            IsWaitingForNextWave = true;
            return;
        }

        SpawnNextWave();
    }

    private void UpgradePowerOnWaveComplete()
    {
        if (!upgradeWaves.Contains(_currentWaveIndex % 5))
            return;

        // If the player has all upgrades, don't upgrade
        if (Player.Instance.PlayerWeaponManager.HasAllUpgrades)
            return;

        // Upgrade the player's power
        UpgradePicker.Instance.Activate();
    }

    private void SpawnNextWave()
    {
        _isBetweenWaves = true;

        // Set the wave as waiting for the next wave
        IsWaitingForNextWave = false;

        // Start the coroutine to wait between waves
        StartCoroutine(WaitBetweenWaves());
    }

    private IEnumerator WaitBetweenWaves()
    {
        var startTime = Time.time;

        TimeBetweenWavesRemaining = timeBetweenWaves;

        // Wait for the time between waves
        while (Time.time - startTime < timeBetweenWaves)
        {
            TimeBetweenWavesRemaining = timeBetweenWaves - (Time.time - startTime);
            yield return null;
        }

        TimeBetweenWavesRemaining = 0;

        // Reset the wave
        _isBetweenWaves = false;

        // Spawn the next wave
        SetWave(CreateRandomEnemyWave(5, 3));
    }

    private void Update()
    {
        // Update the spawn timer
        _batchSpawnTimer.Update(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.P))
            StartGame();
    }

    public void StartGame()
    {
        // Return if the game has already started
        if (HasStartedGame)
            return;

        // Set the game as started
        HasStartedGame = true;

        // Spawn the first wave
        SpawnNextWave();
    }

    public void ResumeGame()
    {
        // Spawn the first wave
        SpawnNextWave();
    }

    #region Wave Controls

    public void SetWave(EnemyWave enemyWave)
    {
        _currentWave = enemyWave;
        _currentWaveBatchIndex = 0;

        // Increment the wave index
        _currentWaveIndex++;

        // Reset the spawn timer
        _batchSpawnTimer.SetMaxTimeAndReset(_currentWave.EnemyBatchInfos[_currentWaveBatchIndex].TimeToNextSpawn);
        _batchSpawnTimer.Start();

        // Reset the enemies in the wave
        _enemiesInWave.Clear();

        // Spawn the first batch
        SpawnNextBatch();

        // Invoke the on wave start event
        onWaveStart?.Invoke();
    }

    public void SpawnNextBatch()
    {
        // If the batch index is greater than the number of batches, stop the timer
        if (_currentWaveBatchIndex >= _currentWave.EnemyBatchInfos.Length)
        {
            _batchSpawnTimer.Stop();
            return;
        }

        // Get the next batch info
        var nextBatchInfo = _currentWave.EnemyBatchInfos[_currentWaveBatchIndex];

        // Increment the batch index
        _currentWaveBatchIndex++;

        // Spawn the batch
        SpawnBatch(nextBatchInfo.EnemyBatch);

        // Set the timer for the next batch
        _batchSpawnTimer.SetMaxTimeAndReset(nextBatchInfo.TimeToNextSpawn);
        _batchSpawnTimer.Start();
    }

    public void SpawnBatch(EnemyBatch enemyBatch)
    {
        // Spawn each enemy in the batch
        foreach (var enemySpawnInfo in enemyBatch.EnemySpawnInfos)
            SpawnEnemy(enemySpawnInfo);
    }

    public void SpawnEnemy(EnemySpawnInfo enemySpawnInfo)
    {
        // Instantiate the enemy prefab at the spawn point
        var enemy = Instantiate(enemySpawnInfo.EnemyPrefab, enemySpawnInfo.SpawnPoint.position, Quaternion.identity);

        // Add the enemy to the list of enemies in the wave
        _enemiesInWave.Add(enemy);

        // Connect to the on death event
        enemy.OnDeath += RemoveEnemyOnEnemyDeath;
        enemy.OnDeath += CheckIfWaveOverOnEnemyDeath;
    }

    private void RemoveEnemyOnEnemyDeath(object sender, HealthChangedEventArgs args)
    {
        // Remove the enemy from the list of enemies in the wave
        _enemiesInWave.Remove(args.Actor as Enemy);
    }

    private void CheckIfWaveOverOnEnemyDeath(object sender, HealthChangedEventArgs args)
    {
        // If there are no enemies left in the wave, invoke the on wave complete event
        if (_enemiesInWave.Count != 0 || _isBetweenWaves || !IsLastBatch)
            return;

        onWaveComplete?.Invoke();
        _isBetweenWaves = true;
    }

    #endregion

    #region Factory Methods

    private EnemySpawnInfo CreateEnemySpawnInfo(Enemy enemyPrefab, Transform spawnPoint)
    {
        return new EnemySpawnInfo(this, enemyPrefab, spawnPoint);
    }

    private EnemySpawnInfo CreateRandomEnemySpawnInfo()
    {
        // Grab a random enemy from the enemy prefabs
        var enemyPrefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];

        // Grab a random spawn point from the spawn points
        var spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        return CreateEnemySpawnInfo(enemyPrefab, spawnPoint);
    }


    private EnemyBatch CreateEnemyBatch(EnemySpawnInfo[] enemySpawnInfos)
    {
        return new EnemyBatch(enemySpawnInfos);
    }

    private EnemyBatch CreateRandomEnemyBatch(int batchSize)
    {
        var enemySpawnInfos = new EnemySpawnInfo[batchSize];

        for (var i = 0; i < batchSize; i++)
            enemySpawnInfos[i] = CreateRandomEnemySpawnInfo();

        return CreateEnemyBatch(enemySpawnInfos);
    }

    private EnemyBatchInfo CreateEnemyBatchInfo(EnemyBatch enemyBatch, float timeToNextSpawn)
    {
        return new EnemyBatchInfo(enemyBatch, timeToNextSpawn);
    }

    private EnemyBatchInfo CreateRandomEnemyBatchInfo(int batchSize)
    {
        var enemyBatch = CreateRandomEnemyBatch(batchSize);
        var timeToNextSpawn = UnityEngine.Random.Range(MIN_RANDOM_TIME_BETWEEN_SPAWNS, MAX_RANDOM_TIME_BETWEEN_SPAWNS);

        return CreateEnemyBatchInfo(enemyBatch, timeToNextSpawn);
    }

    private EnemyWave CreateEnemyWave(EnemyBatchInfo[] enemyBatchInfos)
    {
        return new EnemyWave(enemyBatchInfos);
    }

    private EnemyWave CreateRandomEnemyWave(int batchCount, int batchSize)
    {
        var enemyBatchInfos = new EnemyBatchInfo[batchCount];

        for (var i = 0; i < batchCount; i++)
            enemyBatchInfos[i] = CreateRandomEnemyBatchInfo(batchSize);

        return CreateEnemyWave(enemyBatchInfos);
    }

    #endregion

    #region Debugging

    private void OnDrawGizmos()
    {
        // Draw the spawn points
        for (var i = 0; i < spawnPoints.Length; i++)
        {
            var spawnPoint = spawnPoints[i];

            Gizmos.color = Color.Lerp(Color.red, Color.green, (float)i / spawnPoints.Length);
            Gizmos.DrawSphere(spawnPoint.position, .5f);
        }
    }

    #endregion

    public string GetDebugText()
    {
        return
            $"Wave: {_currentWaveIndex}\n" +
            $"\tEnemies Remaining: {_enemiesInWave.Count}\n" +
            $"\tCurrent Batch: {_currentWaveBatchIndex}\n" +
            $"\tTime: {_batchSpawnTimer.TimeLeft:0.00}";
    }
}