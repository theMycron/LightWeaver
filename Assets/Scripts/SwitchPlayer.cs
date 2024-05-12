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
        /*        activeRobot = robots[0];*/
        ActivateRobot(1);
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
        try
        {
            int robotNumber = int.Parse(keyName);
            //GameObject selectedRobot = robots[robotNumber - 1];
            ActivateRobot(robotNumber);
        }catch(Exception e)
        {
            Debug.Log("Inavlid Robot Number: " + e);
        }

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
        activeRobot = robots[robotNumber - 1];
        PlayerController script = activeRobot.GetComponent<PlayerController>();
        // cant switch to robot if it is disabled
        if (!script.isActive)
        {
            return;
        }
        DisableAllRobots();
        SetCameraTarget();
        activeRobot.GetComponent<Rigidbody>().isKinematic = false;
        //activeRobot.tag = "ActiveRobot";
        script.EnableInput();
    }

    // New method to update the UI image of the selected robot
    private void UpdateRobotImage(int robotNumber)
    {
        foreach (var image in robotImages)
        {
            // Grey out the image
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f); // Adjust the alpha value to make the image semi-transparent
        }

        // Select the new robot
        robotImages[robotNumber].color = Color.white; // Change color to white
    }

    void DisableAllRobots()
    {
        foreach (var robot in robots)
        {
            PlayerController script = robot.GetComponent<PlayerController>();
            robot.GetComponent<Rigidbody>().isKinematic = true;
            script.DisableInput();
            //robot.tag = "Undefined";
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