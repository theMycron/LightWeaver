using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static Cinemachine.CinemachineFreeLook;
using static UnityEngine.Rendering.DebugUI.Table;

public class CubeController : MonoBehaviour
{
    public bool isRaised =false;
    // Flag to track if the cube is raised or not
    [SerializeField]
    public float strength = 5f;
    private Rigidbody rb;
    Transform CubeDes;
    InputManager inputManager;
    GameObject activeRobot;
    [SerializeField] LayerMask CubeLayer;
    [SerializeField] float pickupDistance = 10f;
    float RotationValue;
    [SerializeField] float rotateSpeed =4f;
    private BoxCollider bx;
    private void Awake()
    {
        inputManager = new InputManager();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bx = GetComponent<BoxCollider>();
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
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, CubeLayer))
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
        if (isRaised)
        {
            RotationValue = context.ReadValue<float>() * rotateSpeed * Time.deltaTime;

            if (RotationValue != 0)
            {
                transform.Rotate(Vector3.up * RotationValue);
            }
        }

    }
    public void OnRotateCubeUpPerformed(InputAction.CallbackContext context)
    {
        if (isRaised)
        {
            transform.Rotate(Vector3.right, 180f, Space.Self);
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
        if (isRaised)
        {
            Debug.Log("Placing Cube ");
            rb.useGravity = true;
            rb.drag = 0;
            isRaised = false;
            bx.excludeLayers = LayerMask.GetMask("Nothing");
            activeRobot.GetComponent<PlayerController>().isCarryingObject = false;
        }

    }
    void PickupCube()
    {
        if (!isRaised)
        {
            FindCubePosition();
            float distance = Vector3.Distance(gameObject.transform.position, activeRobot.transform.position);
            Debug.Log("distance: " + distance);
            if (distance > pickupDistance)
            {
                return;
            }
            rb.useGravity = false;
            rb.drag = 12; // drag helps with dampening
            isRaised = true;
            bx.excludeLayers = LayerMask.GetMask("Robot");
            Debug.Log("Cube is Raised" + isRaised);
            Debug.Log("Cube is by:" + activeRobot.name);
            activeRobot.GetComponent<PlayerController>().isCarryingObject = true;
        }

    }
    void FindCubePosition()
    {
        activeRobot = SwitchPlayer.GetActiveRobot();

        CubeDes = activeRobot.transform.Find("CubePosition");
        Debug.Log("Active Robot" + activeRobot.name);
    }

}

