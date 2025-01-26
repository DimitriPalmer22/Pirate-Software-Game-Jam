public struct EnemyWave
{
    private EnemyBatchInfo[] _enemyBatchInfos;
    
    public EnemyBatchInfo[] EnemyBatchInfos => _enemyBatchInfos;
    
    public EnemyWave(EnemyBatchInfo[] enemyBatchInfos)
    {
        _enemyBatchInfos = enemyBatchInfos;
    }
}