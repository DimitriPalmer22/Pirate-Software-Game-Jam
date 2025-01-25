using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField, Min(0)] private float moveSpeed = 8f;
    [SerializeField]private Animator animator;

    #endregion

    #region Private Fields

    private Vector2 _movementInput;

    private Vector3 _aimForward;
    
    private Vector3 _lastPosition;
    private Vector3 _currentVelocity;

    #endregion

    #region Getters

    public Player Player { get; private set; }

    public PlayerInput PlayerInput { get; private set; }


    #endregion

    #region Initialization Functions

    private void Awake()
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
        animator.SetFloat("Speed", _movementInput.magnitude);
    }

    private void OnMovementCanceled(InputAction.CallbackContext obj)
    {
        _movementInput = Vector2.zero;
        animator.SetFloat("Speed", 0);

    }

    #endregion

    #endregion

    #region Update Functions

    private void FixedUpdate()
    {
        
        // // Store current position before movement
        //Vector3 oldPosition = transform.position;

        // Update the player's position
        UpdatePosition();

        // Update the player's rotation
        UpdateRotation();
        // Update the player's animation
    
        // Compute pseudo-velocity based on how far we moved
        //_currentVelocity = (transform.position - oldPosition) / Time.fixedDeltaTime;
    
        UpdateAnimationDirection();
    }

    private void UpdatePosition()
    {
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

        // Move the player
        Player.Rigidbody.MovePosition(Player.Rigidbody.position + movement * Time.fixedDeltaTime);
        //store the velocity       
        Player.Rigidbody.linearVelocity = movement;
    }

    private void UpdateAnimationDirection()
    {
        
        //get the velocity of the player based on movement
        Vector3 velocity = Player.Rigidbody.linearVelocity;
        
        // calculate the dot product between the forward vector and the velocity vector
        float direction = Vector3.Dot(transform.forward, velocity);
        Debug.Log("Dot: " + direction);
        
        //threshold to check if the player is moving
        float threshold = 0.1f;
        float animDirection;

        //check if the player is moving forward or backwards
        if (direction > threshold)
        {
            //positive dot => moving forward
            animDirection = 1;
            Debug.Log("Object is moving Forward");
            animator.SetFloat("Direction", animDirection);
        }
        else if (direction < -threshold)
        {
            animDirection = -1;
            // Negative dot => moving backward
            Debug.Log("Object is moving backward.");
            animator.SetFloat("Direction", animDirection);
        }
        else
        {
            animDirection = 0;
            // Dot is zero => perpendicular or no movement
            Debug.Log("Object is not moving in the forward/backward direction.");
            animator.SetFloat("Direction", animDirection);
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