public struct EnemyWave
{
    private EnemyBatchInfo[] _enemyBatchInfos;
    private readonly bool _isBossWave;

    public EnemyBatchInfo[] EnemyBatchInfos => _enemyBatchInfos;

    public bool IsBossWave => _isBossWave;
    
    public EnemyWave(EnemyBatchInfo[] enemyBatchInfos, bool isBossWave = false)
    {
        _enemyBatchInfos = enemyBatchInfos;
        _isBossWave = isBossWave;
    }
}