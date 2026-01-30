using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed;

    private Controls _controls;
    private Rigidbody2D _rb;
    private float _moveDirection;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        SetupInput();
    }

    void SetupInput()
    {
        _controls = new Controls();
        _controls.Enable();
        _controls.Player.Move.performed += ctx => _moveDirection = ctx.ReadValue<float>();
        _controls.Player.Move.canceled += ctx => _moveDirection = 0;
    }

    private void Move()
    {
        _rb.velocity = new Vector2(_moveDirection * playerSpeed, _rb.velocity.y);
    }

    private void FixedUpdate()
    {
        Move();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
