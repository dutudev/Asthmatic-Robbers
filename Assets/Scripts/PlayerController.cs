using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Mathematics;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private float playerSpeed;
    [SerializeField] private TMP_Text interactText;

    private Controls _controls;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private float _moveDirection;

    private List<Item> interactablesInventory = new List<Item>();
    private List<Item> interactablesNear = new List<Item>();
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
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
            UpdateIteractibles();
        };
        _controls.Player.Interact.performed += ctx => TryInteract();
        _controls.Player.Eject.performed += ctx => EjectItem();
        _controls.Player.SelectItemUp.performed += ctx => UIManager.instance.MoveItemUp();
        _controls.Player.SelectItemDown.performed += ctx => UIManager.instance.MoveItemDown();
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
            if (interactablesNear[0].CompareTag("Item"))
            {
                interactText.text = "Pickup : E"; 
            }else if (interactablesNear[0].CompareTag("Door"))
            {
                interactText.text = "Enter Room : E";
            }else if (interactablesNear[0].CompareTag("Stair") && !interactablesNear[0].GetComponent<SpriteRenderer>().flipX )
            {
                interactText.text = "Go up : E";
            }
            else if (interactablesNear[0].CompareTag("Stair") && interactablesNear[0].GetComponent<SpriteRenderer>().flipX )
            {
                interactText.text = "Go down : E";
            }
            

        }
        else
        {
            interactText.gameObject.SetActive(false);
        }
    }


    public void MoveToPos(Vector2 pos)
    {
        transform.position = pos;
        Camera.main.transform.position = pos + Camera.main.GetComponent<CameraController>().GetOffset();
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
                UIManager.instance.UpdateItems(interactablesInventory);
            }else if (interactablesNear[0].CompareTag("Door"))
            {
                interactablesNear[0].Interact();
            }else if (interactablesNear[0].CompareTag("Stair"))
            {
                interactablesNear[0].Interact();
            }
            UpdateIteractibles();
        }
    }

    private void EjectItem()
    {
        if (interactablesInventory.Count > 0 && UIManager.instance.GetCurrentItem() != -1)
        {
            var itemCur = interactablesInventory[UIManager.instance.GetCurrentItem()];
            //change this for the item currently held + position
            Instantiate(itemCur.GetPrefab(), transform.position + new Vector3(_spriteRenderer.flipX ? -1.25f : 1.25f, 0.5f, 0), quaternion.identity);
            print(itemCur.GetName());
            interactablesInventory.RemoveAt(UIManager.instance.GetCurrentItem());
            UIManager.instance.UpdateItems(interactablesInventory);
            UpdateIteractibles();
            
        }
    }
}
