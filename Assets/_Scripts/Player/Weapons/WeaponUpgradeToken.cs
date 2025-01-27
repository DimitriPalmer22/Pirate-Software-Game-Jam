using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponUpgradeToken
{
    private readonly WeaponScriptableObject _weaponScriptableObject;
    private readonly int _weight;
    private readonly int _upgradeIndex;

    public WeaponScriptableObject WeaponScriptableObject => _weaponScriptableObject;

    public int Weight => _weight;

    public int UpgradeIndex => _upgradeIndex;

    public WeaponUpgradeToken(WeaponScriptableObject weaponScriptableObject, int weight, int upgradeIndex)
    {
        _weaponScriptableObject = weaponScriptableObject;
        _weight = weight;
        _upgradeIndex = upgradeIndex;
    }
    
    public static WeaponUpgradeToken ChooseRandomUpgradeToken(IEnumerable<WeaponUpgradeToken> upgradeTokens)
    {
        // Return if there are no upgrade tokens
        if (upgradeTokens == null || !upgradeTokens.Any())
            return null;
        
        // Sum the total weight
        var totalWeight = upgradeTokens.Sum(upgradeToken => upgradeToken.Weight);

        var randomValue = Random.Range(0, totalWeight);
        
        foreach (var upgradeToken in upgradeTokens)
        {
            randomValue -= upgradeToken.Weight;
            if (randomValue < 0)
                return upgradeToken;
        }

        return null;
    }
}