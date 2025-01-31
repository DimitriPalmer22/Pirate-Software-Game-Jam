using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour, IDebugged
{
    #region Constants

    private const float MIN_RANDOM_TIME_BETWEEN_SPAWNS = 1f;
    private const float MAX_RANDOM_TIME_BETWEEN_SPAWNS = 3f;

    public const int WAVE_CYCLE = 5;

    #endregion

    public static WaveManager Instance { get; private set; }

    #region Serialized Fields

    [SerializeField, Min(0)] private float timeBetweenWaves = 5;

    [SerializeField] private float playerWaveHealAmount = 50;

    [SerializeField] private Enemy bossEnemyPrefab;
    [SerializeField] private Transform bossSpawnPoint;

    [SerializeField] private int[] upgradeWaves = { 2, 4 };
    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Difficulty"), SerializeField] private int diffEnemyAdd = 5;
    [SerializeField] private int diffBatchAdd = 3;

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

    public EnemyWave CurrentWave => _currentWave;
    public EnemyWave NextWave { get; private set; }

    private EnemyWave StandardRandomWave => CreateRandomEnemyWave(
        5 + (int)(diffBatchAdd * Difficulty),
        3 + (int)(diffEnemyAdd * Difficulty)
    );

    public float PlayerWaveHealAmount => playerWaveHealAmount;

    public float Difficulty => _currentWaveIndex / (WAVE_CYCLE * 5f);

    #endregion

    public Action onWaveStart;
    public Action onWaveComplete;

    private void Awake()
    {
        // Initialize the singleton
        Instance = this;

        // Initialize the spawn timer 
        _batchSpawnTimer = new CountdownTimer(10000f);
        _batchSpawnTimer.OnTimerEnd += SpawnNextBatch;
        _batchSpawnTimer.Stop();

        // Add this to the debug manager
        DebugManager.Instance.AddDebuggedObject(this);
    }

    private void OnDestroy()
    {
        // Remove this from the debug manager
        DebugManager.Instance.RemoveDebuggedObject(this);
    }

    private void Start()
    {
        onWaveComplete += SpawnNextWaveOnWaveComplete;
        onWaveComplete += UpgradePowerOnWaveComplete;

        onWaveStart += DisableRendererOnBossWaveStart;
        onWaveComplete += EnableRendererOnBossWaveComplete;
    }

    private void EnableRendererOnBossWaveComplete()
    {
        // Return if this is not a boss wave
        if (!CurrentWave.IsBossWave)
            return;

        // Enable the renderer of this object
        ManekiManager.Instance.ManekiObject.SetActive(true);
    }

    private void DisableRendererOnBossWaveStart()
    {
        // Return if this is not a boss wave
        if (!CurrentWave.IsBossWave)
            return;

        // Disable the renderer of this object
        ManekiManager.Instance.ManekiObject.SetActive(false);
    }

    private void SpawnNextWaveOnWaveComplete()
    {
        // After a boss wave, wait for the player to upgrade
        if (CurrentWave.IsBossWave)
        {
            IsWaitingForNextWave = true;

            // Decrement the wave index to fix the upgrade wave
            _currentWaveIndex--;

            return;
        }

        // TODO: Random waves based on difficulty
        // After every 5 waves, spawn a boss wave
        var nextWave = (_currentWaveIndex % WAVE_CYCLE == 0)
            ? CreateBossWave()
            : StandardRandomWave;

        // Spawn the next wave
        SpawnNextWave(nextWave);
    }

    private void UpgradePowerOnWaveComplete()
    {
        if (!upgradeWaves.Contains(_currentWaveIndex % WAVE_CYCLE))
            return;

        // If the player has all upgrades, don't upgrade
        if (Player.Instance.PlayerWeaponManager.HasAllUpgrades)
            return;

        // Upgrade the player's power
        UpgradePicker.Instance.Activate();
    }

    private void SpawnNextWave(EnemyWave wave)
    {
        _isBetweenWaves = true;

        // Set the wave as waiting for the next wave
        IsWaitingForNextWave = false;

        // Start the coroutine to wait between waves
        StartCoroutine(WaitBetweenWaves(wave));
    }

    private IEnumerator WaitBetweenWaves(EnemyWave wave)
    {
        var startTime = Time.time;

        TimeBetweenWavesRemaining = timeBetweenWaves;

        // Set the next wave
        NextWave = wave;

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
        SetWave(wave);
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
        SpawnNextWave(StandardRandomWave);
    }

    public void ResumeGame()
    {
        // Spawn the first wave
        SpawnNextWave(StandardRandomWave);
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
        enemy.OnDeath += ForceNextBatchOnEnemyDeath;
    }

    private void ForceNextBatchOnEnemyDeath(object sender, HealthChangedEventArgs args)
    {
        // If this is the last batch in the wave, return
        // If there are more enemies left in the wave, return
        if (IsLastBatch || _enemiesInWave.Count > 0)
            return;

        // Force the batch timer to end
        _batchSpawnTimer.ForcePercent(1);
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

    private EnemyWave CreateBossWave()
    {
        var enemyBatchInfos = new List<EnemyBatchInfo>();

        // Add the boss batch
        var bossSpawnInfo = CreateEnemySpawnInfo(bossEnemyPrefab, bossSpawnPoint);
        var bossBatchInfo = CreateEnemyBatchInfo(new EnemyBatch(bossSpawnInfo), .5f);
        enemyBatchInfos.Add(bossBatchInfo);

        // TODO: Add the normal batches
        for (var i = 0; i < _currentWaveIndex / WAVE_CYCLE - 1; i++)
            enemyBatchInfos.Add(CreateRandomEnemyBatchInfo(5));

        return new EnemyWave(enemyBatchInfos.ToArray(), true);
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