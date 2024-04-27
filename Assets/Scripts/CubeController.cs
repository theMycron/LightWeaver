using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static Cinemachine.CinemachineFreeLook;

public class CubeController : MonoBehaviour
{
    public bool isRaised =false; // Flag to track if the cube is raised or not
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
            bx.enabled = false;
            rb.useGravity = false;
            rb.isKinematic = true;
            this.transform.position = CubeDes.position;
            this.transform.parent = CubeDes.transform;
            isRaised = true;
            Debug.Log("Cube is Raised" + isRaised);
        }
        else
        {
            Debug.Log("Placing Cube ");
            this.transform.parent = null;
            rb.useGravity = true;
            rb.isKinematic = false;
            bx.enabled = true;
            isRaised = false;
        }
    }
}

