using System;
using UnityEngine;

public class Player : MonoBehaviour, IActor
{
    #region Serialized Fields
    
    [SerializeField, Min(0)] private float invincibilityDuration = 1f;
    
    #endregion
    
    #region Private Fields

    private CountdownTimer _invincibilityTimer;
    
    #endregion
    
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
        // If the amount is negative, the player is taking damage
        if (amount < 0)
            TakeDamage(-amount, changer, damager, position);

        // If the amount is positive, the player is gaining health
        else if (amount > 0)
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

            // Invoke the OnHealed event
            var args = new HealthChangedEventArgs(this, changer, damager, amount, position);
            OnHealed?.Invoke(this, args);
        }
    }

    private void TakeDamage(float damageAmount, IActor changer, IDamager damager, Vector3 position)
    {
        // Return if the player is invincible
        if (_invincibilityTimer.IsActive)
            return;

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);

        // Invoke the OnDamaged event
        var args = new HealthChangedEventArgs(this, changer, damager, damageAmount, position);
        OnDamaged?.Invoke(this, args);

        if (currentHealth <= 0)
        {
            // Invoke the OnDeath event
            OnDeath?.Invoke(this, args);
        }

        // Start the invincibility timer
        _invincibilityTimer.SetMaxTimeAndReset(invincibilityDuration);
        _invincibilityTimer.Start();

        // // Reset the passive regen timer
        // _passiveRegenTimer.SetMaxTimeAndReset(passiveRegenDelay);
        // _passiveRegenTimer.Start();
    }

    #endregion

    #region Getters

    public GameObject GameObject => gameObject;

    #endregion

    private void Awake()
    {
        // Create the invincibility timer
        _invincibilityTimer = new CountdownTimer(invincibilityDuration);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Update the invincibility timer
        UpdateInvincibility();
    }
    
    private void UpdateInvincibility()
    {
        // Update the invincibility timer
        _invincibilityTimer.Update(Time.deltaTime);
    }
}