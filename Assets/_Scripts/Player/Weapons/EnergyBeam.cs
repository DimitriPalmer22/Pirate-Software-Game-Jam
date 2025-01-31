public class EnergyBeam : RocketLauncher
{
    #region Upgrades

    protected override void CustomUpgrade1()
    {
        bulletSpeed *= 1.25f;
    }

    protected override void CustomUpgrade2()
    {
        bulletSpeed *= 1.25f;
    }

    protected override void CustomUpgrade3()
    {
        baseDamage += 15f;
    }

    protected override void CustomUpgrade4()
    {
        fireRate *= .75f;
    }

    protected override void CustomUpgrade5()
    {
        homingStrength += .25f;
    }

    #endregion
}