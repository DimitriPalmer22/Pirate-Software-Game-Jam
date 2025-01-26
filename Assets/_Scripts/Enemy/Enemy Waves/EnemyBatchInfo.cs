public struct EnemyBatchInfo
{
    private readonly EnemyBatch _enemyBatch;
    private readonly float _timeToNextSpawn;
    
    public EnemyBatch EnemyBatch => _enemyBatch;
    
    public float TimeToNextSpawn => _timeToNextSpawn;

    public EnemyBatchInfo(EnemyBatch enemyBatch, float timeToNextSpawn)
    {
        _enemyBatch = enemyBatch;
        _timeToNextSpawn = timeToNextSpawn;
    }
}