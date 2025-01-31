using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IActor
{
    public static Player Instance { get; private set; }

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
        if (_invincibilityTimer.IsActive && !_invincibilityTimer.IsComplete)
            return;

        // Return if the player is currently dodging
        if (PlayerController.IsDodging)
            return;

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);

        // Invoke the OnDamaged event
        var args = new HealthChangedEventArgs(this, changer, damager, damageAmount, position);
        OnDamaged?.Invoke(this, args);

        // Invoke the OnDeath event
        if (currentHealth <= 0)
            OnDeath?.Invoke(this, args);

        // Start the invincibility timer
        _invincibilityTimer.SetMaxTimeAndReset(invincibilityDuration);
        _invincibilityTimer.Start();
    }

    #endregion

    #region Getters

    public PlayerControls PlayerControls { get; private set; }

    public GameObject GameObject => gameObject;

    public Rigidbody Rigidbody { get; private set; }

    public PlayerController PlayerController { get; private set; }

    public PlayerWeaponManager PlayerWeaponManager { get; private set; }

    public PlayerInteraction PlayerInteraction { get; private set; }

    public bool IsInvincibleBecauseDamaged => _invincibilityTimer.IsActive && _invincibilityTimer.Percentage < 1;

    #endregion

    private void Awake()
    {
        // Set the instance
        Instance = this;

        // Create the invincibility timer
        _invincibilityTimer = new CountdownTimer(invincibilityDuration);

        // Initialize the components
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        PlayerControls = new PlayerControls();
        Rigidbody = GetComponent<Rigidbody>();
        PlayerController = GetComponent<PlayerController>();
        PlayerWeaponManager = GetComponent<PlayerWeaponManager>();
        PlayerInteraction = GetComponent<PlayerInteraction>();
    }

    private void OnEnable()
    {
        PlayerControls.Enable();
    }

    private void OnDisable()
    {
        PlayerControls.Disable();
    }

    private void Start()
    {
        WaveManager.Instance.onWaveComplete += HealOnWaveComplete;
    }

    private void HealOnWaveComplete()
    {
        // Heal after the 3rd wave & the boss wave
        if (WaveManager.Instance.CurrentWaveIndex == 3 || WaveManager.Instance.CurrentWave.IsBossWave)
            ChangeHealth(WaveManager.Instance.PlayerWaveHealAmount, this, null, transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        if (MenuManager.Instance.IsControlsDisabledInMenus)
            PlayerControls.Disable();
        else
            PlayerControls.Enable();

        // Update the invincibility timer
        UpdateInvincibility();
    }

    private void UpdateInvincibility()
    {
        // Update the invincibility timer
        _invincibilityTimer.SetMaxTime(invincibilityDuration);
        _invincibilityTimer.Update(Time.deltaTime);
    }
}