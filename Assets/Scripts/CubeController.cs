using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static Cinemachine.CinemachineFreeLook;

public class CubeController : MonoBehaviour
{
    public bool isRaised =false; // Flag to track if the cube is raised or not
    public float strength = 5f;
    private Rigidbody rb;
    private BoxCollider bx;
    Transform CubeDes;
    InputManager inputManager;
    GameObject robots;
    GameObject activeRobot;
    private void Awake()
    {
        inputManager = new InputManager();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bx = GetComponent<BoxCollider>();
        robots = GameObject.FindGameObjectWithTag("Robots");
    }
    private void OnEnable()
    {
        inputManager.Enable();
        inputManager.Player.MoveCube.performed += OnClickCubePerformed;
    }
    private void OnDisable()
    {
        inputManager.Disable();
    }
    public void OnClickCubePerformed(InputAction.CallbackContext context)
    {
        // Perform a raycast from the mouse cursor position
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        // Perform a raycast from the mouse position
        if (Physics.Raycast(ray, out hit))
            {
            Debug.Log("Collider :" + hit.collider.gameObject);
/*            // Check if the hit collider belongs to the cube GameObject
            if (hit.collider.gameObject == gameObject || hit.collider.gameObject == activeRobot)
            {

                // Handle the click only if the cube itself is clicked
                HandleClick();
            }*/
        }

        HandleClick();

        }

    void setCubePosition()
    {
        if (robots != null)
        {
            Debug.Log("Robots parent found.");
            // Iterate through each child of the robotsParent GameObject
            foreach (Transform robotTransform in robots.transform)
            {
                activeRobot = robotTransform.gameObject;
                // Check if the childObject has the "ActiveRobot" tag
                if (activeRobot.CompareTag("ActiveRobot"))
                {
                    Debug.Log("Found ActiveRobot: " + activeRobot.name);
                    // Do something with the found ActiveRobot
                    CubeDes = activeRobot.transform.Find("CubePosition");
                }
            }
        }
        else
        {
            Debug.LogError("Robots parent GameObject not found.");
        }
    }

    void HandleClick()
    {
        // Check if the left mouse button is clicked
        if (!isRaised)
        {
            setCubePosition();
            // Raise the cube
            //bx.enabled = false;
            rb.useGravity = false;
            //rb.isKinematic = true;
            //this.transform.position = CubeDes.position;
            //  changing transform parent works but causes clipping when rotating robot into a wall
            //this.transform.parent = CubeDes;
            rb.drag = 12; // drag helps with dampening
            isRaised = true;
            Debug.Log("Cube is Raised" + isRaised);
        }
        else
        {
            Debug.Log("Placing Cube ");
            //this.transform.parent = null;
            rb.useGravity = true;
            //rb.isKinematic = false;
            rb.drag = 0;
            //bx.enabled = true;
            isRaised = false;
        }
    }

    private void FixedUpdate()
    {
        // use AddForce to move cube to desired position so that physics collisions work
        if (isRaised)
        {
            Vector3 desiredPosition = CubeDes.position;

            // get the direction to the desired position and multiple by strength variable
            // this will move the cube towards the destination
            Vector3 dirToDesiredPos = (desiredPosition - rb.position).normalized;
            Vector3 forceVector = dirToDesiredPos * strength;

            // apply some dampening if the cube is close to the destination
            float distToDesiredPos = Vector3.Distance(desiredPosition, rb.position)/10;
            distToDesiredPos = Mathf.Clamp01(distToDesiredPos);
            Debug.Log($"Clamped: {distToDesiredPos}, Distance: {Vector3.Distance(desiredPosition, rb.position)}");
            forceVector *= distToDesiredPos;


            //Vector3 forceVector = desiredPosition - Vector3.Lerp(rb.position, desiredPosition, movementSpeed);

            //Debug.Log($"Desired position: {desiredPosition}, RB Position: {rb.position}, Movement Vector: {forceVector}");
            Debug.DrawRay(rb.position, forceVector, Color.blue, 3);
            rb.AddForce(forceVector);
        }
    }
}

