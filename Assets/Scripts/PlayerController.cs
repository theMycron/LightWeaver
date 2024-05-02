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

    [Header("Activation")]
    [SerializeField] public bool isActive = false;
    
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
    [SerializeField] float jumpStartTime;
    float jumpTime;
    bool isJumping;

    [Header("Ground Check")]
    [SerializeField] LayerMask ground;
    [SerializeField] float groundCheckDistance = 0.1f;

    private RobotTextureController texture;
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
        texture = GetComponent<RobotTextureController>();
        ResetJump();
        //set the states at the begining, if isActive == false then disabled
        CheckIfActive();
        
        anim.SetInteger("UpperBodyState", (int)UpperAnimationState.none);

    }

    public void EnableInput()
    {
        // if the robot isn't active don't active the inputmanager
        if (!isActive)
        {
            return;
        }

        InputManager.Enable();

        InputManager.Player.Move.performed += OnMovePerformed;
        InputManager.Player.Move.canceled += OnMoveCancelled;

        InputManager.Player.Jump.performed += OnJumpPerformed;
        InputManager.Player.SuperJump.performed += onSuperJumpPerformed;
    }

    public void DisableInput()
    {
        InputManager.Disable();
        InputManager.Player.Move.performed -= OnMovePerformed;
        InputManager.Player.Move.canceled -= OnMoveCancelled;

        InputManager.Player.Jump.performed -= OnJumpPerformed;
        InputManager.Player.SuperJump.performed -= onSuperJumpPerformed;
    }
    private void Update()   
    {
    }
    private void FixedUpdate()
    {
        // Move the player and get the movement vector
        Vector3 movementVector = MovePlayer();

        // Rotate the player based on movement direction
        RotatePlayer(movementVector);

        RobotFalling();
        RobotLanding();

        rb.drag = IsGrounded() ? groundDrag : 0;

        //SpeedControl();

        //CarryObject();

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
        Vector3 force;
        // Adjust velocity if the player is grounded
        if (IsGrounded() )
        {
            // drag will be applied when grounded
            force = targetVector.normalized * moveSpeed * 10f;

        } else
        {
            // no drag will be applied when airborne (because it messes with the jump height)
            // so limit horizontal movement
            force = targetVector.normalized * moveSpeed  * airMultiplier;
        }

        rb.AddForce(force, ForceMode.Force);
        
        // Return the movement vector
        return targetVector;
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
        
        Transform groundCheck1Trans = gameObject.transform.Find("GroundCheck1");
        Transform groundCheck2Trans = gameObject.transform.Find("GroundCheck2");
        Transform groundCheck3Trans = gameObject.transform.Find("GroundCheck3");
        Transform groundCheck4Trans = gameObject.transform.Find("GroundCheck4");

        bool groundedInCheck1 = Physics.Raycast(groundCheck1Trans.position + verticalOffset, Vector3.down, groundCheckDistance + 0.5f, ground);
        bool groundedInCheck2 = Physics.Raycast(groundCheck2Trans.position + verticalOffset, Vector3.down, groundCheckDistance + 0.5f, ground);
        bool groundedInCheck3 = Physics.Raycast(groundCheck3Trans.position + verticalOffset, Vector3.down, groundCheckDistance + 0.5f, ground);
        bool groundedInCheck4 = Physics.Raycast(groundCheck4Trans.position + verticalOffset, Vector3.down, groundCheckDistance + 0.5f, ground);
        bool groundedInCheck5 = Physics.Raycast(transform.position + verticalOffset, Vector3.down, groundCheckDistance + 0.5f, ground);
        
        return  groundedInCheck1 || groundedInCheck2 || groundedInCheck3 || groundedInCheck4 || groundedInCheck5;

    }
    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log(IsGrounded());
        Jump(isSuperJump: false);
    }
    void onSuperJumpPerformed(InputAction.CallbackContext context)
    {
        Jump(isSuperJump: true);
    }

    void onSuperJumpCancelled(InputAction.CallbackContext context)
    {
        if (isJumping)
        {
            isJumping = false;
        }
        
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    void Jump(bool isSuperJump)
    {
        
            // reset y velocity then apply jump force
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (!isSuperJump && IsGrounded())
            {
                isJumping = true;
                jumpTime = jumpStartTime;
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
            else if (isSuperJump && isJumping)
            {
                if (jumpTime > 0)
                {
                    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                    jumpTime -= Time.deltaTime;
                }
            } else
            {
                isJumping = false;
            }
            anim.SetInteger("BaseState", (int)AnimationState.jumping);

    }


    void RobotFalling()
    {
        // If the player is not grounded, apply gravity
        if (!IsGrounded() && rb.velocity.y <= 0)
        {
            rb.AddForce(Vector3.down * fallSpeed * Time.deltaTime, ForceMode.VelocityChange);
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
            //anim.SetLayerWeight(1, 1f);
            anim.SetInteger("UpperBodyState", (int)UpperAnimationState.carryObject);
            Debug.Log("UpperBodyState" + (int)UpperAnimationState.carryObject);
        }else
        {
            anim.SetInteger("UpperBodyState", (int)UpperAnimationState.none);
            //  layer weight is always set to 1, will do empty animation if not carrying
            //  this is so that the animation transition (carrying > idle) plays 
            //  if layer weight is immediately set to 0, the transition wont be seen
            //anim.SetLayerWeight(1, 0f);
        }
    }

    private void CheckIfActive()
    {
        if (isActive)
        {
            anim.SetInteger("BaseState", (int)AnimationState.idle);
            texture.SetRobotColor(RobotTextureController.ROBOT_GREEN);
        }
        else
        {
            texture.SetRobotColor(RobotTextureController.ROBOT_GREY);
        }
    }

    public void SetCarryingObject(bool value)
    {
        isCarryingObject = value;
        CarryObject();
    }

    public void ActivateRobot()
    {
        isActive = true;
        CheckIfActive();
    }
}
