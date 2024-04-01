using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;



public class PlayerController : MonoBehaviour
{
    public InputManager InputManager;

    public Rigidbody rb;

    Vector2 moveDirection = Vector2.zero;
    [SerializeField] float moveSpeed;

    public Boolean isActive;

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
        InputManager.Enable();
        InputManager.Player.Move.performed += OnMovePerformed;
        InputManager.Player.Move.canceled += OnMoveCancelled;
       

    }

    private void OnDisable()
    {
        InputManager.Disable();
        InputManager.Player.Move.performed -= OnMovePerformed;
        InputManager.Player.Move.canceled -= OnMoveCancelled;
    }
    private void Update()
    {

        
    }
    private void FixedUpdate()
    {
        // Ensure that the moveDirection is normalized so diagonal movement isn't faster
        Vector3 movement = new Vector3(moveDirection.x, 0.0f, moveDirection.y).normalized;

        // Apply movement to the Rigidbody
        rb.velocity = movement * moveSpeed;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }
    private void OnMoveCancelled(InputAction.CallbackContext context)
    {
        moveDirection = Vector2.zero;
    }


}
