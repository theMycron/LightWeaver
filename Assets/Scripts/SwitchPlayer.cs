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
    private GameObject[] robots;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private RobotHUD robotHUD;
    static GameObject activeRobot;
    int activeRobotIndex = 0; // Track the current active robot index


    InputManager inputManager;

    private void Awake()
    {
        inputManager = new InputManager();

        // get children robots
        robots = new GameObject[transform.childCount];
        for (int i = 0; i<robots.Length; i++)
        {
            Transform robot = transform.GetChild(i);
            robots[i] = robot.gameObject;
        }

        robotHUD = FindAnyObjectByType<RobotHUD>();
        
    }
    private void Start()
    {
        /*        activeRobot = robots[0];*/
        ActivateRobot(activeRobotIndex);
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

    public void ActivateRobot(int robotNumber)
    {
        if (robotNumber >= robots.Length || robotNumber < 0)
        {
            return;
        }
        // Update the UI image of the selected robot
        if (robotHUD != null)
            robotHUD.UpdateRobotImage(robotNumber);
        activeRobot = robots[robotNumber];
        PlayerController script = activeRobot.GetComponent<PlayerController>();
        // cant switch to robot if it is disabled
        if (!script.isActive)
        {
            return;
        }
        DisableAllRobots();
        SetCameraTarget();
        script.ChangeControlling(true);
        //activeRobot.tag = "ActiveRobot";
        script.EnableInput();
    }


    void DisableAllRobots()
    {
        foreach (var robot in robots)
        {
            PlayerController script = robot.GetComponent<PlayerController>();
            script.ChangeControlling(false);
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