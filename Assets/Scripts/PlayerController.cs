using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;



public class PlayerController : MonoBehaviour
{
    public InputManager InputManager;

    public Rigidbody rb;

    Vector2 moveDirection = Vector2.zero;
    
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;

    [Header("Camera")]
    [SerializeField] Camera mainCamera;
    
    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown = .2f;
    Boolean readyToJump;

    [Header("Ground Check")]
    [SerializeField] LayerMask ground;
    [SerializeField] float groundCheckDistance = 0.1f;
    private void Awake()
    {
        InputManager = new InputManager();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ResetJump();

    }

    private void OnEnable()
    {
        InputManager.Enable();

        InputManager.Player.Move.performed += OnMovePerformed;
        InputManager.Player.Move.canceled += OnMoveCancelled;

        InputManager.Player.Jump.performed += OnJumpPerformed;


    }

    private void OnDisable()
    {
        InputManager.Disable();
        InputManager.Player.Move.performed -= OnMovePerformed;
        InputManager.Player.Move.canceled -= OnMoveCancelled;

        InputManager.Player.Jump.performed -= OnJumpPerformed;
    }
    private void Update()   
    {

        
    }
    private void FixedUpdate()
    {
        //move the player to target direction
        Vector3 targetVector = new Vector3(moveDirection.x, 0.0f, moveDirection.y);
        var movementVector = MovePlayer(targetVector);

        //rotate the player based on movement direction
        RotatePlayer(movementVector);
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
        
    }
    private void OnMoveCancelled(InputAction.CallbackContext context)
    {
        moveDirection = Vector2.zero;
    }
    Vector3 MovePlayer(Vector3 targetVector)
    {
        //Player Movement Speed
        var speed = moveSpeed * Time.deltaTime;

        targetVector = Quaternion.Euler(0, mainCamera.gameObject.transform.eulerAngles.y, 0) * targetVector;

        var targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
        return targetVector;
    }

    void RotatePlayer(Vector3 movementVector)
    {
        if(movementVector.magnitude == 0)
        {
            return;
        }
        var rotation = Quaternion.LookRotation(movementVector);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed);
    }

    bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red, 2, true);
        // transform.position is at the very bottom of the robot
        // add a vertical offset to the raycast position to avoid creating it inside the ground
        Vector3 verticalOffset = new Vector3(0, 0.5f, 0);
        return Physics.Raycast(transform.position + verticalOffset, Vector3.down, groundCheckDistance + 0.5f, ground);
    }
    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        Debug.Log(IsGrounded());

        if (readyToJump && IsGrounded())
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }



}
