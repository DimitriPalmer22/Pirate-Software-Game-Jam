using System;
using UnityEngine;

public class HealthChangedEventArgs : EventArgs
{
    public IActor Actor { get; }
    public IActor Changer { get; }

    public IDamager DamagerObject { get; }
    public float Amount { get; }

    public Vector3 Position { get; }


    public HealthChangedEventArgs(IActor actor, IActor changer, IDamager damager, float amount, Vector3 position)
    {
        Actor = actor;
        Changer = changer;
        DamagerObject = damager;
        Amount = amount;
        Position = position;
    }
}