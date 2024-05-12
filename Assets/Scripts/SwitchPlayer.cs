using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static Cinemachine.CinemachineFreeLook;

public class SwitchPlayer : MonoBehaviour
{

    private GameObject[] robots;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    static GameObject activeRobot; 


    InputManager inputManager;
    private void Awake()
    {
        inputManager = new InputManager();
        PlayerController[] controllers = GetComponentsInChildren<PlayerController>();
        robots = new GameObject[controllers.Length];
        for (int i = 0;i<robots.Length;i++)
        {
            robots[i] = controllers[i].gameObject;
        }
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

    }

    void ActivateRobot(int robotNumber)
    {
        if (robotNumber > robots.Length || activeRobot == robots[robotNumber - 1])
        {
            return;
        }
        GameObject requestedRobot = robots[robotNumber - 1];
        PlayerController script = requestedRobot.GetComponent<PlayerController>();
        // cant switch to robot if it is disabled
        if (!script.isActive)
        {
            return;
        }
        activeRobot = requestedRobot;
        DisableAllRobots();
        SetCameraTarget();
        activeRobot.GetComponent<Rigidbody>().isKinematic = false;
        //activeRobot.tag = "ActiveRobot";
        script.EnableInput();
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
