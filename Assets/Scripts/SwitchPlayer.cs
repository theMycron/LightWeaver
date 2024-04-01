using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Cinemachine.CinemachineFreeLook;

public class SwitchPlayer : MonoBehaviour
{

    [SerializeField] private GameObject[] robots;

    GameObject activeRobot; 

    InputManager inputManager;
    private void Awake()
    {
        inputManager = new InputManager();
    }
    private void Start()
    {
       activeRobot = robots[0];
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
    }

    private void OnSwitchRobotPerformed(InputAction.CallbackContext context)
    {
        string keyName = context.control.displayName;
        Debug.Log("Pressed key: " + keyName);
        int robotNumber = int.Parse(keyName);
        ActivateRobot(robotNumber);
    }

    void ActivateRobot(int robotNumber)
    {
        DisableAllRobots();
        activeRobot = robots[robotNumber - 1];
        PlayerController script = activeRobot.GetComponent<PlayerController>();
        script.enabled = true;
    }
    void DisableAllRobots()
    {
        foreach (var robot in robots)
        {
            PlayerController script = robot.GetComponent<PlayerController>();
            script.enabled = false;
        }
    }
}
