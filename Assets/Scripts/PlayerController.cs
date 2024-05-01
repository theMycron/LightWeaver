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
    [SerializeField] public float rotateSpeed;
    [SerializeField] float groundDrag;

    [Header("Camera")]
    [SerializeField] Camera mainCamera;
    
    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown = .2f;
    [SerializeField] float fallSpeed = 50f;
    [SerializeField] float airMultiplier = .2f;
    Boolean readyToJump;

    [Header("Ground Check")]
    [SerializeField] LayerMask ground;
    [SerializeField] float groundCheckDistance = 0.1f;


    private Animator anim;
    private bool isFalling;
    public bool isCarryingObject;
    private enum AnimationState
    {
        disabled = 0,
        idle = 1,
        walking = 2,
        landing = 3,
        jumping = 4,
        falling = 5,
    }
    private enum UpperAnimationState
    {
        none = 0,
        carryObject = 1,
        carryRobot = 2
    }
    private enum LowerAnimationState
    {
        none = 0,
        carriedByRobot = 1
    }


    private void Awake()
    {
        InputManager = new InputManager();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        ResetJump();
        //set the states at the begining
        anim.SetInteger("BaseState", (int)AnimationState.idle);
        anim.SetInteger("UpperBodyState", (int)UpperAnimationState.none);

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
        if (IsGrounded())
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        SpeedControl();
    }
    private void FixedUpdate()
    {
        // Move the player and get the movement vector
        Vector3 movementVector = MovePlayer();

        // Rotate the player based on movement direction
        RotatePlayer(movementVector);

        RobotFalling();
        RobotLanding();
        CarryObject();

/*        //check if player is not moving
        if (moveDirection == Vector2.zero && IsGrounded())
        {
            anim.SetInteger("BaseState", (int)AnimationState.idle);
        }*/
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
        anim.SetInteger("BaseState", (int)AnimationState.walking);

    }
    private void OnMoveCancelled(InputAction.CallbackContext context)
    {
        moveDirection = Vector2.zero;
        anim.SetInteger("BaseState", (int)AnimationState.idle);
    }
    Vector3 MovePlayer()
    {
        // Calculate movement vector
        Vector3 targetVector = new Vector3(moveDirection.x, 0.0f, moveDirection.y);
        targetVector = Quaternion.Euler(0, mainCamera.gameObject.transform.eulerAngles.y, 0) * targetVector;

        // Adjust velocity if the player is grounded
        if (IsGrounded() )
        {
            rb.AddForce(targetVector.normalized * moveSpeed * 10f, ForceMode.Force);

        } else
        {
            rb.AddForce(targetVector.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
        
        // Return the movement vector
        return targetVector;
    }
    void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }
    void RotatePlayer(Vector3 movementVector)
    {
        if (movementVector.magnitude == 0)
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
        //Debug.Log(IsGrounded());

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
        anim.SetInteger("BaseState", (int)AnimationState.jumping);
    }


    void RobotFalling()
    {
        // If the player is not grounded, apply gravity
        if (!IsGrounded() && rb.velocity.y <= 0)
        {
            rb.velocity += Vector3.down * fallSpeed * Time.deltaTime;
            anim.SetInteger("BaseState", (int)AnimationState.falling);
            isFalling = true;
        }
    }

    void RobotLanding()
    {
        if (isFalling && IsGrounded())
        {
            // only play the landing animation if the player isnt trying to move
            // if the player is trying to move, play the walking animation
            if (moveDirection == Vector2.zero)
                anim.SetInteger("BaseState", (int)AnimationState.landing);
            else
                 anim.SetInteger("BaseState", (int)AnimationState.walking);
            isFalling = false;
        }
    }

    void CarryObject()
    {
        
        if (isCarryingObject)
        {
            anim.SetLayerWeight(1, 1f);
            anim.SetInteger("UpperBodyState", (int)UpperAnimationState.carryObject);
            Debug.Log("UpperBodyState" + (int)UpperAnimationState.carryObject);
        }else
        {
            anim.SetInteger("UpperBodyState", (int)UpperAnimationState.none);
            anim.SetLayerWeight(1, 0f);
        }
    }
}
