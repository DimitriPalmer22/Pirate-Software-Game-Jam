using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IActor
{
    private static HashSet<Enemy> _allEnemies = new();
    
    public static IReadOnlyCollection<Enemy> AllEnemies => _allEnemies;
    
    #region IActor

    [SerializeField, Min(0)] private float maxHealth;
    [SerializeField, Min(0)] private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public event HealthChangedEventHandler OnDamaged;
    public event HealthChangedEventHandler OnHealed;
    public event HealthChangedEventHandler OnDeath;

    public void ChangeHealth(float amount, IActor changer, IDamager damager, Vector3 position)
    {
        // Clamp the health value between 0 and the max health
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        HealthChangedEventArgs args;

        // If the amount is less than 0, invoke the OnDamaged event
        if (amount < 0)
        {
            args = new HealthChangedEventArgs(this, changer, damager, -amount, position);
            OnDamaged?.Invoke(this, args);
        }

        // If the amount is greater than 0, invoke the OnHealed event
        else if (amount > 0)
        {
            args = new HealthChangedEventArgs(this, changer, damager, amount, position);
            OnHealed?.Invoke(this, args);
        }

        // If the amount is 0, do nothing
        else
            return;

        // If the enemy's health is less than or equal to 0, call the Die function
        if (currentHealth <= 0)
        {
            // Invoke the OnDeath event
            OnDeath?.Invoke(this, args);

            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    #endregion

    public GameObject GameObject => gameObject;

    private void Awake()
    {
        // Set the current health to the max health
        currentHealth = maxHealth;
        
        // Add this enemy to the list of all enemies
        _allEnemies.Add(this);
        
        OnDeath += (sender, args) => _allEnemies.Remove(this);
    }
    
    private void OnDestroy()
    {
        // Remove this enemy from the list of all enemies
        _allEnemies.Remove(this);
    }

}