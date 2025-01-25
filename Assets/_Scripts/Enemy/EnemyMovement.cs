using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField, Min(0)] private float navigationUpdateInterval = .25f;
    
    private NavMeshAgent _navMeshAgent;

    private void Awake()
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
        // Start the FollowPlayer coroutine
        StartCoroutine(FollowPlayer());
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private IEnumerator FollowPlayer()
    {
        while (true)
        {
            // If there is no player instance, wait for the next frame
            if (Player.Instance == null)
            {
                yield return null;
                continue;
            }

            // Set the destination of the NavMeshAgent to the player's position
            SetDestination(Player.Instance.transform.position);

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