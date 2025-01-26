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

    private float _localVelX;
    private float _localVelZ;
    
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
        
        //temp: set the speed of the player based on the movement input
        animator.SetFloat("Speed", _movementInput.magnitude);
    }

    private void OnMovementCanceled(InputAction.CallbackContext obj)
    {
        _movementInput = Vector2.zero;
        
        //temp: set the speed of the player to zero
        animator.SetFloat("Speed", 0);

    }

    #endregion

    #endregion

    #region Update Functions

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
        
        //set the velocity       
        Player.Rigidbody.linearVelocity = movement;
    }


    // Update the animation direction of the player
    private void UpdateAnimationDirection()
    {
        //get the velocity of the player in world space
        Vector3 worldVelocity = Player.Rigidbody.linearVelocity;
        
        //convert the velocity to local space
        Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);
        
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
        float direction = Vector3.Dot(transform.forward, worldVelocity);
        Debug.Log("Dot: " + direction);
        
        //threshold to check if the player is moving
        float threshold = 0.1f;

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