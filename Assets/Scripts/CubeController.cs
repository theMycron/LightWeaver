
using UnityEngine;
using UnityEngine.InputSystem;

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


    [Header("Cube Movement")]
    [SerializeField] public float strength = 0f;
    Transform CubeDes;
    
    [Header("Cube Rotation")]
    [SerializeField] float rotateSpeed = 4f;
    float RotationValue;
    private float horizontalRotation = 90f;


    [Header("Pickup/Place Cube")]
    [SerializeField] float horizontalPickupDistance = 10f;
    [SerializeField] float verticalPickupDistance = 7f;
    [SerializeField] float cubeMassWhenPlaced = 10000f;
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
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, cubeLayer))
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
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, cubeLayer))
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

            if (RotationValue != 0 && horizontalRotation != -90)
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
            if (EnvSFX.instance != null)
                EnvSFX.instance.PlayObjectSFX(EnvSFX.instance.cubePickup);
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
        if (!IsActiveRobotGrounded()) { Debug.Log("robot not grounded"); return; } 
        FindCubePosition();
        if (!IsRobotNearCube()) return;

        if (EnvSFX.instance != null)
            EnvSFX.instance.PlayObjectSFX(EnvSFX.instance.cubePickup);
        rb.mass = 1;
        rb.useGravity = false;
        rb.drag = 12; // drag helps with dampening
        isRaised = true;
/*        this.transform.position = CubeDes.position;*/
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
    bool IsActiveRobotGrounded()
    {
        activeRobot = SwitchPlayer.GetActiveRobot();
        var playerScript = activeRobot.GetComponent<PlayerController>();
        return playerScript.IsGrounded();
    }
    void FindCubePosition()
    {
        activeRobot = SwitchPlayer.GetActiveRobot();
        CubeDes = activeRobot.transform.Find("CubePosition");
        Debug.Log("Active Robot" + activeRobot.name);
    }

    bool IsGrounded()
    {
        Vector3 boxSize = new Vector3(cubeHeight, cubeHeight, cubeHeight); // Define the size of the box
        float boxHalfHeight = boxSize.y * 0.5f; // Half of the height of the box
        Vector3 boxCenter = transform.position; // The center of the box

        Debug.DrawRay(transform.position, Vector3.down * (boxHalfHeight + 0.2f), Color.red);

        // Perform the box cast
        bool placedOnGround = Physics.BoxCast(boxCenter, boxSize * 0.5f, Vector3.down, Quaternion.identity, boxHalfHeight + 0.2f, groundLayer);
/*        bool placedOnCube = Physics.BoxCast(boxCenter, boxSize * 0.5f, Vector3.down, Quaternion.identity, boxHalfHeight + 0.2f, cubeLayer);
        bool placedOnLaserCube = Physics.BoxCast(boxCenter, boxSize * 0.5f, Vector3.down, Quaternion.identity, boxHalfHeight + 0.2f, laserCubeLayer);*/

        return placedOnGround /*|| placedOnCube || placedOnLaserCube*/;
    }
    /*    bool IsGrounded()
        {
            Debug.DrawRay(transform.position, Vector3.down * (cubeHeight * 0.5f + 0.2f), Color.red);

            bool placedOnGround = Physics.Raycast(transform.position, Vector3.down, cubeHeight * 0.5f + 0.2f, groundLayer);
            *//*        bool placedOnCube = Physics.Raycast(transform.position, Vector3.down, cubeHeight * 0.5f + 0.2f, cubeLayer);
                    bool placedOnLaserCube = Physics.Raycast(transform.position, Vector3.down, cubeHeight * 0.5f + 0.2f, laserCubeLayer);*//*

            return placedOnGround *//*|| placedOnCube || placedOnLaserCube*//*;
        }*/

    /*    bool IsRobotNearCube()
        {
            float distance = Vector3.Distance(gameObject.transform.position, activeRobot.transform.position);
            Debug.Log("distance: " + distance);
            return distance <= horizontalPickupDistance;
        }*/
    bool IsRobotNearCube()
    {
        Vector3 cubePosition = gameObject.transform.position;
        Vector3 robotPosition = activeRobot.transform.position;

        // Calculate the horizontal distance X-Z
        float horizontalDistance = Vector3.Distance(new Vector3(cubePosition.x, 0, cubePosition.z), new Vector3(robotPosition.x, 0, robotPosition.z));

        // Calculate the vertical distance
        float verticalDistance = Mathf.Abs(cubePosition.y - robotPosition.y);


        Debug.Log("Horizontal Distance: " + horizontalDistance);
        Debug.Log("Vertical Distance: " + verticalDistance);

        // Check if both the horizontal and vertical distances are within their respective pickup distances
        return horizontalDistance <= horizontalPickupDistance && verticalDistance <= verticalPickupDistance;
    }
    /*    void CubeFalling()
        {
            if (isFalling && rb.velocity.y <= 0)
            {
                rb.AddForce(Vector3.down * fallingSpeed * Time.deltaTime, ForceMode.VelocityChange);
            }
        }*/

}

