using System;
using System.Collections.Generic;

public struct EnemyBatch
{
    private readonly EnemySpawnInfo[] _enemySpawnInfos;

    public IReadOnlyList<EnemySpawnInfo> EnemySpawnInfos => _enemySpawnInfos;
    
    public EnemyBatch(params EnemySpawnInfo[] enemySpawnInfos)
    {
        // Copy the array
        _enemySpawnInfos = new EnemySpawnInfo[enemySpawnInfos.Length];
        Array.Copy(enemySpawnInfos, _enemySpawnInfos, enemySpawnInfos.Length);
    }
}