using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;



public class PlayerController : MonoBehaviour, IActivable, ILaserInteractable
{
    public InputManager InputManager;

    public Rigidbody rb;

    Vector2 moveDirection = Vector2.zero;

    [Header("Activation")]
    [SerializeField] public bool isActive = false;
    [SerializeField] public int robotNumber;
    
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] public float rotateSpeed;
    [SerializeField] float groundDrag;
    [SerializeField] float maxSpeed = 4f;

    [Header("Camera")]
    [SerializeField] Camera mainCamera;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    float jumpForceCounter;
    [SerializeField] float fallSpeed = 50f;
    [SerializeField] float JumpTime;
    [SerializeField] float minJumpTime;
    float jumpTimeCounter;
    float minJumpTimeLimit;
    bool isJumping = false;

    [Header("Ground Check")]
    [SerializeField] LayerMask ground;
    [SerializeField] float groundCheckDistance = 0.1f;

    private RobotTextureController texture;
    private Animator anim;
    private bool isFalling;
    private bool isJumpCancelled = false;
    private bool isCarryingObject;
    private bool isRotating;
    private bool isLaserColliding = false;
    private LaserColors currentLaserColor = LaserColors.red;
    private GameObject laserHitBy;
    
    [Header("Laser Pointing")]
    private bool isRobotPointing;
    [SerializeField] GameObject startingPoint;
    private Laser laserScript;
    private Vector3 mousePosition;

    [SerializeField] LayerMask robotLayer;
    Vector3 requiredHitPoint;

    [SerializeField] MultiAimConstraint headAim;

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
        laserScript = GetComponent<Laser>();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        texture = GetComponent<RobotTextureController>();
        minJumpTimeLimit = JumpTime - minJumpTime;
        //set the states at the begining, if isActive == false then disabled
        CheckIfActive();

        anim.SetInteger("UpperBodyState", (int)UpperAnimationState.none);
        mousePosition = Vector3.zero;

        //anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        //anim.SetIKPosition(AvatarIKGoal.RightHand, Vector3.zero);
        //anim.SetIKPosition(AvatarIKGoal.LeftHand, Vector3.up);
        
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

        InputManager.Player.Jump.started += OnJumpStarted;
        InputManager.Player.Jump.performed += OnJumpPerformed;
        InputManager.Player.Jump.canceled += OnJumpCancelled;

        InputManager.Player.RotateRobot.started += OnRotateRobotStarted;
        InputManager.Player.RotateRobot.canceled += OnRotateRobotCancelled;
    }

    public void DisableInput()
    {
        InputManager.Disable();
        InputManager.Player.Move.performed -= OnMovePerformed;
        InputManager.Player.Move.canceled -= OnMoveCancelled;

        InputManager.Player.Jump.started -= OnJumpStarted;
        InputManager.Player.Jump.performed -= OnJumpPerformed;
        InputManager.Player.Jump.canceled -= OnJumpCancelled;

        InputManager.Player.RotateRobot.started -= OnRotateRobotStarted;
        InputManager.Player.RotateRobot.canceled -= OnRotateRobotCancelled;

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

        /*        rb.drag = IsGrounded() ? groundDrag : 0;*/

        EnsurePlayerIsNotMovingAtSpeedOfLight();

        //SpeedControl();

        //CarryObject();

        if (isJumping)
        {
            Jump();
        }

        if (IsGrounded() && moveDirection == Vector2.zero)
        {
            //rotate robot when press/hold right click
            if (isRotating)
                SetMouseRotatePosition();
            HandleLaserPointing();
        } else
        {
            SetRobotPointing(false);
        }

        

    }

    private void EnsurePlayerIsNotMovingAtSpeedOfLight()
    {
        if (rb.isKinematic)
            return;
        float xSpeed = Mathf.Abs(rb.velocity.x);
        float zSpeed = Mathf.Abs(rb.velocity.z);

        if(xSpeed > maxSpeed)
        {
            xSpeed = maxSpeed;
        }

        if (zSpeed > maxSpeed)
        {
            zSpeed = maxSpeed;
        }

        rb.velocity = new Vector3(
            Mathf.Sign(rb.velocity.x) * xSpeed,
            rb.velocity.y,
            Mathf.Sign(rb.velocity.z) * zSpeed
        );
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
        force = targetVector.normalized * moveSpeed * 10f;

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
        // transform.position is at the very bottom of the robot
        // add a vertical offset to the raycast position to avoid creating it inside the ground
        Vector3 verticalOffset = new Vector3(0, 0.5f, 0);
        LayerMask layersToCheck = (1 << 6) | (1 << 7) | (1 << 9);

        Transform groundCheck1Trans = gameObject.transform.Find("GroundCheck1");
        Transform groundCheck2Trans = gameObject.transform.Find("GroundCheck2");
        Transform groundCheck3Trans = gameObject.transform.Find("GroundCheck3");
        Transform groundCheck4Trans = gameObject.transform.Find("GroundCheck4");

        bool groundedInCheck1 = Physics.Raycast(groundCheck1Trans.position + verticalOffset, Vector3.down, groundCheckDistance + verticalOffset.y, layersToCheck);
        bool groundedInCheck2 = Physics.Raycast(groundCheck2Trans.position + verticalOffset, Vector3.down, groundCheckDistance + verticalOffset.y, layersToCheck);
        bool groundedInCheck3 = Physics.Raycast(groundCheck3Trans.position + verticalOffset, Vector3.down, groundCheckDistance + verticalOffset.y, layersToCheck);
        bool groundedInCheck4 = Physics.Raycast(groundCheck4Trans.position + verticalOffset, Vector3.down, groundCheckDistance + verticalOffset.y, layersToCheck);
        bool groundedInCheck5 = Physics.Raycast(transform.position + verticalOffset, Vector3.down, groundCheckDistance + verticalOffset.y, layersToCheck);

        Debug.DrawRay(groundCheck1Trans.position + verticalOffset, Vector3.down * (groundCheckDistance + verticalOffset.y), Color.red, 0.1f, true);
        Debug.DrawRay(groundCheck2Trans.position + verticalOffset, Vector3.down * (groundCheckDistance + verticalOffset.y), Color.red, 0.1f, true);
        Debug.DrawRay(groundCheck3Trans.position + verticalOffset, Vector3.down * (groundCheckDistance + verticalOffset.y), Color.red, 0.1f, true);
        Debug.DrawRay(groundCheck4Trans.position + verticalOffset, Vector3.down * (groundCheckDistance + verticalOffset.y), Color.red, 0.1f, true);
        Debug.DrawRay(transform.position + verticalOffset, Vector3.down * (groundCheckDistance + verticalOffset.y), Color.red, 2, true);

        return groundedInCheck1 || groundedInCheck2 || groundedInCheck3 || groundedInCheck4 || groundedInCheck5;

    }
    void OnJumpStarted(InputAction.CallbackContext context)
    {
        //Debug.Log("Jump Started!!!");
        if (IsGrounded() && !isJumping)
        {
            isJumping = true;
            isJumpCancelled = false;
            anim.SetInteger("BaseState", (int)AnimationState.jumping);
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            jumpTimeCounter = JumpTime;
            jumpForceCounter = jumpForce;

            //Debug.Log("JumpStart Y Velocity: " + rb.velocity.y);
            //Debug.Log("JumpStart Y Position: " + rb.position.y);
        }
    }
    void OnJumpPerformed(InputAction.CallbackContext context)
    {
    }
    void OnJumpCancelled(InputAction.CallbackContext context)
    {
        if (!isJumping)
        {
            return;
        }

        //Debug.Log("Jump attempted Cancel!!!");
        //Debug.Log("JumpTimeCounter:" + jumpTimeCounter);
        //Debug.Log("Robot Y Velocity:" + rb.velocity.y);
        //Debug.Log("Robot is Grounded:" + IsGrounded());

        //if (!isFalling && jumpTimeCounter < minJumpTimeLimit)
        //{
        //    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //    isJumping = false;
        //}
        //if (IsGrounded())
        //{
        //    /*            anim.SetInteger("BaseState", (int)AnimationState.idle);*/
        //    rb.AddForce(transform.up * 13f, ForceMode.Impulse);
        //}
        //if (jumpTimeCounter >= 0.75 * JumpTime)
        //{
        //    rb.AddForce(transform.up * 6f, ForceMode.Impulse);
        //    Debug.Log("small Jump Performed: ");
        //}
        isJumpCancelled = true;
    }

    void OnRotateRobotStarted(InputAction.CallbackContext context)
    {
        isRotating = true;
    }

    void OnRotateRobotCancelled(InputAction.CallbackContext context)
    {
        isRotating = false;
    }

    // sets the robot rotation to wherever the mouse is
    // used to rotate robot on right click
    void SetMouseRotatePosition()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = mainCamera.ScreenPointToRay(mouse);
        RaycastHit hit;

        if (Physics.Raycast(castPoint, out hit,Mathf.Infinity,~robotLayer))
        {
            Vector3 directionToMouse = (hit.point - transform.position).normalized;
            directionToMouse.y = 0f;
            transform.rotation = Quaternion.LookRotation(directionToMouse);
        }
    }
    
    void Jump()
    {
        // if player attempted to cancel jump, dont stop the jump until minimum time limit
        if (isJumpCancelled && !isFalling && jumpTimeCounter < minJumpTimeLimit)
        {
            StopJump();
        }
        if (jumpTimeCounter > 0)
        {
            /*rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);*/
            rb.AddForce(transform.up* jumpForceCounter, ForceMode.Impulse);
            jumpForceCounter *= 0.5f;
            jumpTimeCounter -= Time.deltaTime;
            //Debug.Log("Jumping! time: " + jumpTimeCounter + ", force: " + jumpForceCounter);
        }
        else if (jumpTimeCounter <= 0 || jumpForceCounter < 0.01f)
        {
            // stop jump if reached maximum jump time 
            StopJump();
        }
    }
    void StopJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        jumpTimeCounter = 0;
        isJumpCancelled = false;
        isJumping = false;
        //Debug.Log("Cancelled jump! started falling at jumpTime: " + jumpTimeCounter + "  " + minJumpTimeLimit);
    }
    void RobotFalling()
    {
        // If the player is not grounded, apply gravity
        if (!IsGrounded() && rb.velocity.y <= 0)
        {
            //Debug.Log("Robot is Falling!!!");
            rb.AddForce(Vector3.down * fallSpeed * Time.deltaTime, ForceMode.VelocityChange);
            anim.SetInteger("BaseState", (int)AnimationState.falling);
            //Debug.Log("BaseState"+ (int)AnimationState.falling);
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
            {
                //Debug.Log("Robot is Landing!!!");
                anim.SetInteger("BaseState", (int)AnimationState.landing);
                //Debug.Log("BaseState"+ (int)AnimationState.landing);
                /*                anim.SetInteger("BaseState", (int)AnimationState.idle);*/
            }
                
            else
                anim.SetInteger("BaseState", (int)AnimationState.walking);
            isFalling = false;
        }
    }

    public void LaserCollide(Laser sender)
    {
        if (isLaserColliding || laserHitBy != null) return;
        // laser pointing logic
        currentLaserColor = sender.colorEnum;
        isLaserColliding = true;
        laserHitBy = sender.gameObject;
        switch (sender.colorEnum)
        {
            case LaserColors.red:
                texture.SetRobotColor(RobotTextureController.ROBOT_RED); break;
            case LaserColors.blue:
                texture.SetRobotColor(RobotTextureController.ROBOT_BLUE); break;
        }
    }

    public void LaserExit(Laser sender)
    {
        if (!isLaserColliding || laserHitBy != sender.gameObject) return;
        isLaserColliding = false;
        laserHitBy = null;
        Debug.Log("Laser exited. isRobotPointing: " + isRobotPointing);
        SetRobotPointing(false);
        texture.SetRobotColor(RobotTextureController.ROBOT_GREEN);
    }

    private void HandleLaserPointing()
    {
        if (!isLaserColliding || isCarryingObject) return;

        if (!isRobotPointing && isRotating)
        {
            SetRobotPointing(true);
        }
        Debug.Log("Handling Laser pointing. isRobotPointing: " + isRobotPointing);

        // get mosue position
        RaycastHit hit;
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = mainCamera.ScreenPointToRay(mouse);

        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity) && isRotating)
        {
            mousePosition = hit.point;
            // set head and hand target positions based on where the mouse clicks
            laserScript.direction = (hit.point - startingPoint.transform.position).normalized;
            headAim.data.sourceObjects[0].transform.position = hit.point;
            Debug.DrawRay(hit.point, Vector3.up*2, Color.green, 1);
        }

    }

    void CarryObject()
    {

        if (isCarryingObject)
        {
            //anim.SetLayerWeight(1, 1f);
            anim.SetInteger("UpperBodyState", (int)UpperAnimationState.carryObject);
            SetRobotPointing(false);
            //Debug.Log("UpperBodyState" + (int)UpperAnimationState.carryObject);
        }
        else
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

    public void Activate(Component sender)
    {
        if (!isActive)
        {
            isActive = true;
            CheckIfActive();
        }
    }

    public void Deactivate(Component sender)
    {
        // robot cannot be deactivated
    }
    public void Activate(Component sender, int objectNumber, string targetName, object data)
    {
        if (!isActive && CheckRobotNumber(objectNumber) && targetName == "Robot")
        {
            isActive = true;
            CheckIfActive();
        }
    }

    private bool CheckRobotNumber(int robotNumber)
    {
        return this.robotNumber == robotNumber;
    }

    public bool IsRobotCarryingObject()
    {
        return isCarryingObject;
    }


    private void OnAnimatorIK()
    {
        // aiming hand at target IK logic

        if (isRobotPointing)
        {
            Debug.Log("Setting IK positions");
            // set right hand to look at mouse
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKPosition(AvatarIKGoal.RightHand, mousePosition);
        } else
        {
            Debug.Log("Resetting IK positions");
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        }

    }


    private void SetRobotPointing(bool value)
    {
        laserScript.enabled = value;
        if (value)
            laserScript.SetLaserColor(currentLaserColor);    
        headAim.weight = value ? 1 : 0;
        isRobotPointing = value;
    }

    private void OnDestroy()
    {
        DisableInput();
    }
}
