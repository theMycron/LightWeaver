using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static Cinemachine.CinemachineFreeLook;

public class SwitchPlayer : MonoBehaviour
{
    [SerializeField] private GameObject[] robots;
    [SerializeField] private Image[] robotImages;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    static GameObject activeRobot;
    int activeRobotIndex = 0; // Track the current active robot index

    InputManager inputManager;

    private void Awake()
    {
        inputManager = new InputManager();
    }

    private void Start()
    {
        ActivateRobot(activeRobotIndex);
        UpdateRobotImagesVisibility();
    }

    private void OnEnable()
    {
        inputManager.Enable();
        inputManager.Player.SwitchRobot.performed += OnSwitchRobotPerformed;
    }

    private void OnDisable()
    {
        inputManager.Disable();
        inputManager.Player.SwitchRobot.performed -= OnSwitchRobotPerformed;
    }

    private void OnSwitchRobotPerformed(InputAction.CallbackContext context)
    {
        string keyName = context.control.displayName;
        Debug.Log("Pressed key: " + keyName);

        if (keyName.Equals("E"))
        {
            // Switch to the next robot
            SwitchToNextRobot();
        }
        else if (keyName.Equals("Q"))
        {
            // Switch to the previous robot
            SwitchToPreviousRobot();
        }
        else
        {
            try
            {
                int robotNumber = int.Parse(keyName);
                ActivateRobot(robotNumber - 1);
            }
            catch (Exception e)
            {
                Debug.Log("Invalid Robot Number: " + e);
            }
        }
    }

    void SwitchToNextRobot()
    {
        activeRobotIndex++;
        if (activeRobotIndex >= robots.Length)
            activeRobotIndex = 0;

        ActivateRobot(activeRobotIndex);
    }

    void SwitchToPreviousRobot()
    {
        activeRobotIndex--;
        if (activeRobotIndex < 0)
            activeRobotIndex = robots.Length - 1;

        ActivateRobot(activeRobotIndex);
    }

    void ActivateRobot(int robotNumber)
    {
        if (robotNumber >= robots.Length || robotNumber < 0)
        {
            return;
        }

        // Update the UI image of the selected robot
        UpdateRobotImage(robotNumber);
        activeRobot = robots[robotNumber];
        PlayerController script = activeRobot.GetComponent<PlayerController>();

        // Can't switch to robot if it is disabled
        if (!script.isActive)
        {
            return;
        }

        DisableAllRobots();
        SetCameraTarget();
        activeRobot.GetComponent<Rigidbody>().isKinematic = false;
        script.EnableInput();
    }

    // Method to update the UI image of the selected robot
    private void UpdateRobotImage(int robotNumber)
    {
        for (int i = 0; i < robotImages.Length; i++)
        {
            if (i < robots.Length)
            {
                // Grey out the image
                robotImages[i].color = new Color(robotImages[i].color.r, robotImages[i].color.g, robotImages[i].color.b, 0.5f); // Adjust the alpha value to make the image semi-transparent
            }
            else
            {
                // Hide the image completely
                robotImages[i].gameObject.SetActive(false);
            }
        }

        // Select the new robot
        robotImages[robotNumber].color = Color.white; // Change color to white
    }

    // Method to update the visibility of robot images based on the number of robots
    private void UpdateRobotImagesVisibility()
    {
        for (int i = 0; i < robotImages.Length; i++)
        {
            if (i < robots.Length)
            {
                robotImages[i].gameObject.SetActive(true);
            }
            else
            {
                robotImages[i].gameObject.SetActive(false);
            }
        }
    }

    void DisableAllRobots()
    {
        foreach (var robot in robots)
        {
            PlayerController script = robot.GetComponent<PlayerController>();
            robot.GetComponent<Rigidbody>().isKinematic = true;
            script.DisableInput();
        }
    }

    void SetCameraTarget()
    {
        virtualCamera.Follow = activeRobot.transform;
    }

    public static GameObject GetActiveRobot()
    {
        return activeRobot;
    }
}