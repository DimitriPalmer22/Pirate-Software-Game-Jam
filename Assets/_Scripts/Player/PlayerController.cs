using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField, Min(0)] private float moveSpeed = 8f;

    #endregion

    #region Private Fields

    private Vector2 _movementInput;

    private Vector3 _aimForward;

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
    }

    private void OnMovementCanceled(InputAction.CallbackContext obj)
    {
        _movementInput = Vector2.zero;
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