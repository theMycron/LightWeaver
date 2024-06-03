using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static Cinemachine.CinemachineFreeLook;
using System.Linq;

public class SwitchPlayer : MonoBehaviour
{
     private GameObject[] robots;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private bool followRobot;


    static GameObject activeRobot; 
    private RobotHUD robotHUD;

    int activeRobotIndex = -1; // Track the current active robot index

    public int RobotCount { get { return robots.Length; } }

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
    }
    private void Start()
    {
        /*        activeRobot = robots[0];*/
        robotHUD = FindAnyObjectByType<RobotHUD>();
        if (robotHUD != null )
            robotHUD.InitializeHUD(RobotCount);
        ActivateRobot(0);
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
        int nextRobotIndex = activeRobotIndex + 1;
        if (nextRobotIndex >= robots.Length)
            nextRobotIndex = 0;
        ActivateRobot(nextRobotIndex);
    }

    void SwitchToPreviousRobot()
    {
        int previousRobotIndex = activeRobotIndex - 1;
        if (previousRobotIndex < 0)
            previousRobotIndex = robots.Length - 1;
        ActivateRobot(previousRobotIndex);
    }

    public void ActivateRobot(int robotNumber)
    {
        if (robotNumber >= robots.Length || robotNumber < 0)
        {
            return;
        }

        // cant switch to robot if it is disabled
        PlayerController script = robots[robotNumber].GetComponent<PlayerController>();
        if (!script.isActive)
        {
            return;
        }

        if (robotNumber == activeRobotIndex)
            return;

        // Update the UI image of the selected robot
        if (robotHUD != null)
            robotHUD.UpdateRobotImage(robotNumber);

        activeRobot = robots[robotNumber];

        if (EnvSFX.instance != null)
            EnvSFX.instance.PlayObjectSFX(EnvSFX.instance.robotSwitch);

        activeRobotIndex = robotNumber;
        DisableAllRobots();
        SetCameraTarget();
        script.ChangeControlling(true);
        //activeRobot.tag = "ActiveRobot";

    }


    void DisableAllRobots()
    {
        foreach (var robot in robots)
        {
            PlayerController script = robot.GetComponent<PlayerController>();
            script.ChangeControlling(false);
            //robot.tag = "Undefined";
        }
    }

    void SetCameraTarget()
    {
        if (!followRobot) return;
        virtualCamera.Follow = activeRobot.transform;
    }

    public static GameObject GetActiveRobot()
    {
        return activeRobot;
    }
}