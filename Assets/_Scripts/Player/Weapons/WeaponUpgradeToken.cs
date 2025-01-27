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
}