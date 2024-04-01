using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public InputManager InputManager;

    private InputAction move;
    private InputAction jump;
    private InputAction switchRobot;
    
    private Rigidbody rb;

     Vector2 moveDirection = Vector2.zero;
    [SerializeField] float moveSpeed;

    private void Awake()
    {
        InputManager = new InputManager();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        move = InputManager.Player.Move;
        move.Enable();

        jump = InputManager.Player.Jump;
        jump.Enable();

        switchRobot = InputManager.Player.SwitchRobot;
        switchRobot.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        switchRobot.Disable();
    }
    private void Update()
    {
        moveDirection = move.ReadValue<Vector2>();

    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, 0, moveDirection.y * moveSpeed);
    }
}
