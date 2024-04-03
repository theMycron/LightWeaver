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

   

    [SerializeField] Camera camera;
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

        InputManager.Player.Jump.performed += OnJumpPerformed;
        InputManager.Player.Jump.canceled += OnJumpCancelled;


    }

    private void OnDisable()
    {
        InputManager.Disable();
        InputManager.Player.Move.performed -= OnMovePerformed;
        InputManager.Player.Move.canceled -= OnMoveCancelled;

        InputManager.Player.Jump.performed -= OnJumpPerformed;
        InputManager.Player.Jump.canceled -= OnJumpCancelled;
    }
    private void Update()   
    {

        
    }
    private void FixedUpdate()
    {
        Vector3 targetVector = new Vector3(moveDirection.x, 0.0f, moveDirection.y);
        MovePlayer(targetVector);
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
        
    }
    private void OnMoveCancelled(InputAction.CallbackContext context)
    {
        moveDirection = Vector2.zero;
    }
    void MovePlayer(Vector3 targetVector)
    {
        //Player Movement Speed
        var speed = moveSpeed * Time.deltaTime;

        targetVector = Quaternion.Euler(0, camera.gameObject.transform.eulerAngles.y, 0) * targetVector;

        var targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
    }
    void OnJumpPerformed(InputAction.CallbackContext context)
    {

    }
    void OnJumpCancelled(InputAction.CallbackContext context)
    {

    }


}
