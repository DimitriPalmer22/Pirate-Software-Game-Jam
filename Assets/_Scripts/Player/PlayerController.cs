using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player), typeof(PlayerInput))]
public class PlayerController : ComponentScript<Player>
{
    private static readonly int SpeedAnimationID = Animator.StringToHash("Speed");

    #region Serialized Fields

    [SerializeField, Min(0)] private float moveSpeed = 8f;
    [SerializeField] private Animator animator;

    [Header("Dodge")] [SerializeField, Min(0)]
    private float dodgeSpeed = 20f;

    [SerializeField, Min(0)] private float dodgeDuration = 20f;
    [SerializeField, Min(0)] private float dodgeCooldown = 1;

    #endregion

    #region Private Fields

    private Vector2 _movementInput;

    private Vector3 _aimForward;

    private float _localVelX;
    private float _localVelZ;

    private bool _isDodging;
    private CountdownTimer _dodgeCooldownTimer;

    #endregion

    #region Getters

    public Player Player { get; private set; }

    public PlayerInput PlayerInput { get; private set; }
    
    public bool IsDodging => _isDodging;

    #endregion

    #region Initialization Functions

    protected override void CustomAwake()
    {
        // Initialize the components
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        // Get the player component
        Player = GetComponent<Player>();
        PlayerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        // Initialize the input
        InitializeInput();

        // Initialize the dodge cooldown timer
        _dodgeCooldownTimer = new CountdownTimer(dodgeCooldown, true, true);
        _dodgeCooldownTimer.Start();
    }


    #region Input Functions

    private void InitializeInput()
    {
        // Movement input
        Player.PlayerControls.Player.Move.performed += OnMovementPerformed;
        Player.PlayerControls.Player.Move.canceled += OnMovementCanceled;

        // Rotation Input
        Player.PlayerControls.Player.AimMouse.performed += OnAimMousePerformed;
        Player.PlayerControls.Player.AimStick.performed += OnAimStickPerformed;

        // Dash Input
        Player.PlayerControls.Player.Dodge.performed += OnDodgePerformed;
    }

    private void OnDodgePerformed(InputAction.CallbackContext obj)
    {
        // Start the dodge coroutine
        StartCoroutine(DodgeCoroutine());
    }

    private IEnumerator DodgeCoroutine()
    {
        // Return if the player is already dodging
        if (_isDodging)
            yield break;

        // Return if the dodge cooldown timer is not complete
        if (!_dodgeCooldownTimer.IsComplete)
            yield break;

        // Set the dodge flag
        _isDodging = true;

        // Store the current movement vector and normalize it
        var movement = Player.Rigidbody.linearVelocity;

        // If the movement vector is zero, set it to the player's forward direction
        if (movement == Vector3.zero)
            movement = transform.forward;

        movement.y = 0;
        movement.Normalize();

        // Store the start time
        var startTime = Time.time;

        while (Time.time - startTime < dodgeDuration)
        {
            // Set the player's velocity to the dodge speed
            Player.Rigidbody.linearVelocity = movement * dodgeSpeed;

            yield return null;
        }

        // Reset the dodge flag 
        _isDodging = false;

        // Reset the dodge cooldown timer
        _dodgeCooldownTimer.SetMaxTimeAndReset(dodgeCooldown);
    }

    private void OnAimStickPerformed(InputAction.CallbackContext obj)
    {
        // Change the player's rotation to the stick position
        var stickPosition = obj.ReadValue<Vector2>();

        // Get the camera from the camera manager
        var mainCam = CameraManager.Instance.MainCam;

        // Relate the stick position to the camera's forward direction
        var camForward = mainCam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        var camRight = mainCam.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        var aimForward = camForward * stickPosition.y + camRight * stickPosition.x;

        // Set the aim forward
        _aimForward = aimForward;
    }

    private void OnAimMousePerformed(InputAction.CallbackContext obj)
    {
        // Change the player's rotation to the mouse position
        var mousePosition = obj.ReadValue<Vector2>();

        // Get the camera from the camera manager
        var mainCam = CameraManager.Instance.MainCam;

        // Convert the point from screen space to world space
        var ray = mainCam.ScreenPointToRay(mousePosition);

        // Check if the ray hits something
        var lookAtPoint = Physics.Raycast(ray, out var hit)
            ? hit.point
            : ray.GetPoint(100f);

        lookAtPoint.y = transform.position.y;

        // Get the forward direction
        var forward = lookAtPoint - transform.position;
        forward.y = 0;

        // Set the aim forward
        _aimForward = forward.normalized;
    }

    private void OnMovementPerformed(InputAction.CallbackContext obj)
    {
        _movementInput = obj.ReadValue<Vector2>();

        //temp: set the speed of the player based on the movement input
        animator.SetFloat(SpeedAnimationID, _movementInput.magnitude);
    }

    private void OnMovementCanceled(InputAction.CallbackContext obj)
    {
        _movementInput = Vector2.zero;

        //temp: set the speed of the player to zero
        animator.SetFloat(SpeedAnimationID, 0);
    }

    #endregion

    #endregion

    #region Update Functions

    private void Update()
    {
        // Update the dodge cooldown timer
        _dodgeCooldownTimer.SetMaxTime(dodgeCooldown);
        _dodgeCooldownTimer.Update(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // Update the player's position
        UpdatePosition();

        // Update the player's rotation
        UpdateRotation();

        // Update the player's animation
        UpdateAnimationDirection();
    }

    private void UpdatePosition()
    {
        // Return if the player is dodging
        if (_isDodging)
            return;

        // Get the camera from the camera manager
        var mainCamTransform = CameraManager.Instance.MainCam.transform;

        // Calculate the movement vector in the camera's forward direction
        var camForward = mainCamTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        var camRight = mainCamTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        var movement = (camForward * _movementInput.y + camRight * _movementInput.x) * moveSpeed;

        // Set the velocity       
        Player.Rigidbody.linearVelocity = movement;
    }


    // Update the animation direction of the player
    private void UpdateAnimationDirection()
    {
        //get the velocity of the player in world space
        var worldVelocity = Player.Rigidbody.linearVelocity;

        //convert the velocity to local space
        var localVelocity = transform.InverseTransformDirection(worldVelocity);

        //set the local velocity
        _localVelX = localVelocity.x;
        _localVelZ = localVelocity.z;

        //normalize the local velocity
        _localVelX /= moveSpeed;
        _localVelZ /= moveSpeed;

        //feed the local velocity to the animator
        animator.SetFloat("VelX", _localVelX, 0.1f, Time.deltaTime);
        animator.SetFloat("VelZ", _localVelZ, 0.1f, Time.deltaTime);

        //clamp the local velocity
        _localVelX = Mathf.Clamp(_localVelX, -1, 1);
        _localVelZ = Mathf.Clamp(_localVelZ, -1, 1);

        // calculate the dot product between the forward vector and the velocity
        var direction = Vector3.Dot(transform.forward, worldVelocity);
        // Debug.Log("Dot: " + direction);

        //threshold to check if the player is moving
        var threshold = 0.1f;

        //check if the player is moving forward or backwards
        if (direction > threshold)
        {
            //positive dot => moving forward
            Debug.Log("Object is moving Forward");
        }
        else if (direction < -threshold)
        {
            // Negative dot => moving backward
            Debug.Log("Object is moving backward.");
        }
        else
        {
            // Dot is zero => perpendicular or no movement
            Debug.Log("Object is not moving in the forward/backward direction.");
        }
    }

    private void UpdateRotation()
    {
        // Set the player's rotation to the aim forward
        if (_aimForward == Vector3.zero)
            return;

        var rotation = Quaternion.LookRotation(_aimForward);
        Player.Rigidbody.MoveRotation(rotation);
    }

    #endregion

    #region Debugging

    private void OnDrawGizmos()
    {
        const float lineLength = 10f;

        // Draw the forward vector
        Debug.DrawRay(transform.position, transform.forward * lineLength, Color.blue);
    }

    #endregion
}