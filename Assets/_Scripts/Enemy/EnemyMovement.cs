using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : ComponentScript<Enemy>
{
    #region Serialized Fields

    [SerializeField] private Animator _animator;
    [SerializeField, Min(0)] private float navigationUpdateInterval = .25f;

    [Header("Difficulty"), SerializeField] private float diffSpeedAdd = 4;

    #endregion

    private NavMeshAgent _navMeshAgent;

    protected override void CustomAwake()
    {
        // Initialize the components
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        // Get the NavMeshAgent component
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Apply the difficulty
        ApplyDifficulty();

        // Start the FollowPlayer coroutine
        StartCoroutine(FollowPlayer());
    }

    private void ApplyDifficulty()
    {
        // Add the difficulty speed to the speed of the NavMeshAgent
        _navMeshAgent.speed += diffSpeedAdd * WaveManager.Instance.Difficulty;
    }

    private IEnumerator FollowPlayer()
    {
        // Follow the player while the player's health is greater than 0
        while (Player.Instance.CurrentHealth > 0)
        {
            // If there is no player instance, wait for the next frame
            if (Player.Instance == null)
            {
                yield return null;
                continue;
            }

            // Set the destination of the NavMeshAgent to the player's position
            SetDestination(Player.Instance.transform.position);
            _animator.SetBool("isMoving", _navMeshAgent.velocity.magnitude > 0);

            // Wait for a second
            yield return new WaitForSeconds(navigationUpdateInterval);
        }
    }
    

    private void SetDestination(Vector3 destination)
    {
        // Return if the agent is null or disabled
        if (_navMeshAgent == null || !_navMeshAgent.enabled)
            return;

        // Set the destination of the NavMeshAgent to the destination
        _navMeshAgent.SetDestination(destination);
    }
}