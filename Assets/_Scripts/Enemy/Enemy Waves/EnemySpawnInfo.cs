using UnityEngine;

/// <summary>
/// A single enemy spawned at a single spawn point
/// </summary>
public struct EnemySpawnInfo
{
    private readonly WaveManager _waveManager;
    private readonly Enemy _enemyPrefab;
    private readonly Transform _spawnPoint;

    public WaveManager WaveManager => _waveManager;
    public Enemy EnemyPrefab => _enemyPrefab;
    public Transform SpawnPoint => _spawnPoint;

    public EnemySpawnInfo(WaveManager waveManager, Enemy enemyPrefab, Transform spawnPoint)
    {
        _waveManager = waveManager;
        _enemyPrefab = enemyPrefab;
        _spawnPoint = spawnPoint;
    }
}