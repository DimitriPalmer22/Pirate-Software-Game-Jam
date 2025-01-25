using System;
using UnityEngine;

public delegate void HealthChangedEventHandler(object sender, HealthChangedEventArgs args);

public interface IActor : IInterfacedObject
{
    public float CurrentHealth { get; }
    public float MaxHealth { get; }
    
    public event HealthChangedEventHandler OnDamaged;
    public event HealthChangedEventHandler OnHealed;
    public event HealthChangedEventHandler OnDeath;

    public void ChangeHealth(float amount, IActor changer, IDamager damager, Vector3 position);
}