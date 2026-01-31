using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Mathematics;

public class PlayerController : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    [SerializeField] private float playerSpeed;
    [SerializeField] private TMP_Text interactText;

    private Controls _controls;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private float _moveDirection;

    private List<Item> interactablesInventory = new List<Item>();
    private List<Item> interactablesNear = new List<Item>();

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        SetupInput();
    }

    void SetupInput()
    {
        _controls = new Controls();
        _controls.Enable();
        _controls.Player.Move.performed += ctx => 
        {
            _moveDirection = ctx.ReadValue<float>();
            _spriteRenderer.flipX = _moveDirection < 0;
            _animator.SetBool(IsMoving, true);
            UpdateIteractibles();
        };
        _controls.Player.Move.canceled += ctx =>
        {
            _animator.SetBool(IsMoving, false);
            _moveDirection = 0;
        };
        _controls.Player.Interact.performed += ctx => TryInteract();
        _controls.Player.Eject.performed += ctx => EjectItem();
    }

    private void Move()
    {
        _rb.velocity = new Vector2(_moveDirection * playerSpeed, _rb.velocity.y);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        if (interactablesNear.Count > 0)
        {
            interactText.gameObject.transform.position = interactablesNear[0].transform.position + new Vector3(0, interactablesNear[0].GetTextHeight());
        }
    }
    
    public void AddToList(GameObject item)
    {
        interactablesNear.Add(item.GetComponent<Item>());
        UpdateIteractibles();
    }

    public void RemoveFromList(GameObject item)
    {
        interactablesNear.Remove(item.GetComponent<Item>());
        UpdateIteractibles();
    }

    private void UpdateIteractibles()
    {
        interactablesNear.Sort((x, y) => (Mathf.Abs(transform.position.x - x.transform.position.x)).CompareTo(Mathf.Abs(transform.position.x - y.transform.position.x)));
        if (interactablesNear.Count > 0)
        {
            interactText.gameObject.SetActive(true);
            //add different text for object / door / stairs etc.
            interactText.text = "Pickup : E";

        }
        else
        {
            interactText.gameObject.SetActive(false);
        }
    }

    


    private void TryInteract()
    {
        if (interactablesNear.Count > 0)
        {
            var itemCur = interactablesNear[0];
            if (interactablesNear[0].CompareTag("Item"))
            {
                interactablesNear.RemoveAt(0);
                interactablesInventory.Add(itemCur);
                itemCur.Interact();
                
            }
            UpdateIteractibles();
        }
    }

    private void EjectItem()
    {
        if (interactablesInventory.Count > 0)
        {
            var itemCur = interactablesInventory[0];
            //change this for the item currently held + position
            Instantiate(itemCur.GetPrefab(), transform.position + new Vector3(_spriteRenderer.flipX ? -1.25f : 1.25f, 0.5f, 0), quaternion.identity);
            print(itemCur.GetName());
            interactablesInventory.RemoveAt(0);
            UpdateIteractibles();
            
        }
    }
}
