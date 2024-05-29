using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static Cinemachine.CinemachineFreeLook;
using static UnityEngine.Rendering.DebugUI.Table;

public class CubeController : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollider bx;
    InputManager inputManager;

    public bool isRaised =false;
    bool isFalling = false;

    [Header("Ground Check")]
    [SerializeField] float cubeHeight;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask cubeLayer;
    [SerializeField] LayerMask laserCubeLayer;


    [Header("Cube Movement")]
    [SerializeField] public float strength = 0f;
    Transform CubeDes;
    
    [Header("Cube Rotation")]
    [SerializeField] float rotateSpeed = 4f;
    float RotationValue;
    private float horizontalRotation = 90f;


    [Header("Pickup/Place Cube")]
    [SerializeField] float pickupDistance = 10f;
    [SerializeField] float cubeMassWhenPlaced = 1000f;
    GameObject activeRobot;
    GameObject robotRaisingCube;

    
    private void Awake()
    {
        inputManager = new InputManager();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bx = GetComponent<BoxCollider>();
        rb.mass = cubeMassWhenPlaced;
    }
    private void FixedUpdate()
    {
        // use AddForce to move cube to desired position so that physics collisions work
        if (isRaised)
        {
            /*            FindCubePosition();*/
            Vector3 desiredPosition = CubeDes.position;

            // get the direction to the desired position and multiple by strength variable
            // this will move the cube towards the destination
            Vector3 dirToDesiredPos = (desiredPosition - rb.position).normalized;
            Vector3 forceVector = dirToDesiredPos * strength;

            // apply some dampening if the cube is close to the destination
            float distToDesiredPos = Vector3.Distance(desiredPosition, rb.position) / 10;
            distToDesiredPos = Mathf.Clamp01(distToDesiredPos);
            /*            Debug.Log($"Clamped: {distToDesiredPos}, Distance: {Vector3.Distance(desiredPosition, rb.position)}");*/
            forceVector *= distToDesiredPos;

            //Vector3 forceVector = desiredPosition - Vector3.Lerp(rb.position, desiredPosition, movementSpeed);

            //Debug.Log($"Desired position: {desiredPosition}, RB Position: {rb.position}, Movement Vector: {forceVector}");
            Debug.DrawRay(rb.position, forceVector, Color.blue, 3);
            rb.AddForce(forceVector * strength);
        }
        IsGrounded();
        if (isFalling && IsGrounded())
        {
            Debug.Log("cube is grounded");
            isFalling = false;
            rb.mass = cubeMassWhenPlaced;
            rb.AddForce(transform.forward * 10f, ForceMode.Acceleration);
        }

        
    }
    private void OnEnable()
    {
        inputManager.Enable();
        inputManager.Player.MoveCube.performed += OnMoveCubePerformed;
        inputManager.Player.RotateCube.performed += OnRotateCubePerformed;
        inputManager.Player.RotateCubeUp.performed += OnRotateCubeUpPerformed;
    }
    private void OnDisable()
    {
        inputManager.Disable();
        inputManager.Player.MoveCube.performed -= OnMoveCubePerformed;
        inputManager.Player.RotateCube.performed -= OnRotateCubePerformed;
        inputManager.Player.RotateCubeUp.performed -= OnRotateCubeUpPerformed;
    }
    public void OnMoveCubePerformed(InputAction.CallbackContext context)
    {
        //if Mouse left Button is clicked perform Raycast Check
        if (context.control.name == "leftButton")
        {
            // Perform a raycast from the mouse cursor position
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            // Perform a raycast from the mouse position
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, cubeLayer + laserCubeLayer))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    // Handle the click only if the cube itself is clicked
                    HandleClick();
                }
            }
        } else
        {
            PlaceCube();
        }
    }
    public void OnRotateCubePerformed(InputAction.CallbackContext context)
    {
        if (!IsActiveRobotCarryingObject() && !isRaised)
        {
            // Perform a raycast from the mouse cursor position
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            // Perform a raycast from the mouse position
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, cubeLayer + laserCubeLayer))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    //check if robot is near cube
                    if (IsRobotNearCube())
                    {
                        RotateCube();
                    }

                }
            }
        } else if ( isRaised && activeRobot == robotRaisingCube)
        {
            RotateCube();
        }

    }
    void RotateCube()
    {
            RotationValue = inputManager.Player.RotateCube.ReadValue<float>() * rotateSpeed * Time.deltaTime;

            if (RotationValue != 0)
            {
                transform.Rotate(Vector3.up * RotationValue);
            }
        
    }
    public void OnRotateCubeUpPerformed(InputAction.CallbackContext context)
    {
        activeRobot = SwitchPlayer.GetActiveRobot();
        if (isRaised && activeRobot == robotRaisingCube)
        {
            transform.Rotate(Vector3.left, horizontalRotation, Space.Self);
            horizontalRotation *= -1;
        }
    }

    void HandleClick()
    {
        if (isRaised)
        {
            PlaceCube();
        }
        else
        {
            PickupCube();
        }
    }
    void PlaceCube()    
    {
        activeRobot = SwitchPlayer.GetActiveRobot();
        if (isRaised && activeRobot == robotRaisingCube)
        {
            Debug.Log("Placing Cube ");
            
            rb.useGravity = true;
            rb.drag = 0;
            rb.velocity = new Vector3(0f,rb.velocity.y,0f);
            isRaised = false;
            strength = 0f;
            isFalling = true;
            bx.excludeLayers = LayerMask.GetMask("Nothing");
            activeRobot.GetComponent<PlayerController>().SetCarryingObject(false);
            
        }

    }
    void PickupCube()
    {
        if (isRaised) return;
        if (IsActiveRobotCarryingObject()) return;
        FindCubePosition();
        if (!IsRobotNearCube()) return;

        rb.mass = 1;
        rb.useGravity = false;
        rb.drag = 12; // drag helps with dampening
        isRaised = true;
        this.transform.position = CubeDes.position;
        //Invoke(nameof(AddForce), 0.5f);
        strength = 75;
        bx.excludeLayers = LayerMask.GetMask("Robot");
        Debug.Log("Cube is Raised" + isRaised);
        Debug.Log("Cube is by:" + activeRobot.name);
        activeRobot.GetComponent<PlayerController>().SetCarryingObject(true);
        robotRaisingCube = activeRobot;
    }
    void AddForce()
    {
        strength = 75;
    }
    bool IsActiveRobotCarryingObject()
    {
        activeRobot = SwitchPlayer.GetActiveRobot();
        var playerScript = activeRobot.GetComponent<PlayerController>();
        return playerScript.IsRobotCarryingObject();
    }
    void FindCubePosition()
    {
        activeRobot = SwitchPlayer.GetActiveRobot();
        CubeDes = activeRobot.transform.Find("CubePosition");
        Debug.Log("Active Robot" + activeRobot.name);
    }

    bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector3.down * (cubeHeight * 0.5f + 0.2f), Color.red);

        bool placed = Physics.Raycast(transform.position, Vector3.down, cubeHeight * 0.5f + 0.2f, groundLayer + cubeLayer + laserCubeLayer);

        return placed;
    }

    bool IsRobotNearCube()
    {
        float distance = Vector3.Distance(gameObject.transform.position, activeRobot.transform.position);
        Debug.Log("distance: " + distance);
        return distance <= pickupDistance;
    }

/*    void CubeFalling()
    {
        if (isFalling && rb.velocity.y <= 0)
        {
            rb.AddForce(Vector3.down * fallingSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }
    }*/

}

