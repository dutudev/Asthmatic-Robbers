using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Mathematics;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float playerSpeed;
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private SpriteRenderer maskSpriteRenderer;
    [SerializeField] private ValueFinalCounter valueFinalCounter;

    private Controls _controls;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private float _moveDirection;
    private bool _hasMask = false;
    private bool _isExposed = false;
    private float _stamina = 1f, _exposure = 1f;
    private float _maxWeight = 8.5f, _allWeight;
    private float _lastInhale = 0;
    
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
        _controls.Player.Move.performed += ctx => PerformedMove(ctx);
        _controls.Player.Move.canceled += ctx => CanceledMove(ctx);
        _controls.Player.Interact.performed += ctx => TryInteract();
        _controls.Player.Eject.performed += ctx => EjectItem();
        _controls.Player.SelectItemUp.performed += ctx => UIManager.instance.MoveItemUp();
        _controls.Player.SelectItemDown.performed += ctx => UIManager.instance.MoveItemDown();
        _controls.Player.Mask.performed += ctx => ToggleMask();
    }

    private void PerformedMove(InputAction.CallbackContext ctx)
    {
        _moveDirection = ctx.ReadValue<float>();
        _spriteRenderer.flipX = _moveDirection < 0;
        maskSpriteRenderer.flipX = _spriteRenderer.flipX;
        _animator.SetBool(IsMoving, true);
        UpdateIteractibles();
    }

    private void CanceledMove(InputAction.CallbackContext ctx)
    {
        _animator.SetBool(IsMoving, false);
        _moveDirection = 0;
        UpdateIteractibles();
    }

    private void Move()
    {
        float t = 1f - _allWeight / _maxWeight;
        if (_lastInhale > Time.time)
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
            return;
        }
        _rb.velocity = new Vector2(_moveDirection * Mathf.Lerp(playerSpeed / 1.5f, playerSpeed, t), _rb.velocity.y + 0.05f);
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

        float staminaDecrease = 60;
        if (_hasMask)
        {
            staminaDecrease = 10;
        }

        _stamina -= 1 / staminaDecrease * Time.deltaTime;

        if (_isExposed && !_hasMask)
        {
            _exposure -= 1f / 5f * Time.deltaTime;
        }
        
        UIManager.instance.UpdateStatsImages(_exposure, _stamina);

        if (_exposure <= 0)
        {
            UIManager.instance.ShowMenu(0, 0);
        }
        if (_stamina <= 0)
        {
            UIManager.instance.ShowMenu(0, 1);
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
                if (interactablesNear[0].GetWeight() + _allWeight > _maxWeight)
                {
                    UIManager.instance.ShowMaxReach();
                    return;
                }
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
            UpdateWeight();
        }
    }

    private void EjectItem()
    {
        if (interactablesInventory.Count > 0 && UIManager.instance.GetCurrentItem() != -1)
        {
            var itemCur = interactablesInventory[UIManager.instance.GetCurrentItem()];
            //change this for the item currently held + position
            var item = Instantiate(itemCur.GetPrefab(), transform.position + new Vector3(_spriteRenderer.flipX ? -1.25f : 1.25f, 0.5f, 0), quaternion.identity);
            item.GetComponent<Item>().SetObjectData(interactablesInventory[UIManager.instance.GetCurrentItem()].GetDataRuntime());
            //print(itemCur.GetName());
            interactablesInventory.RemoveAt(UIManager.instance.GetCurrentItem());
            UIManager.instance.UpdateItems(interactablesInventory);
            UpdateIteractibles();
            UpdateWeight();
        }
        else if(_lastInhale < Time.time)
        {
            _lastInhale = Time.time + 1f;
            _stamina += 0.6f;
            _stamina = Mathf.Clamp01(_stamina);
            _exposure -= 0.05f;
        }
    }

    private void UpdateWeight()
    {
        _allWeight = 0;
        foreach (var item in interactablesInventory)
        {
            _allWeight += item.GetWeight();
        }
    }

    private void ToggleMask()
    {
        _hasMask = !_hasMask;
        maskSpriteRenderer.gameObject.SetActive(_hasMask);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Entry"))
        {
            MoveToPos(new Vector2(0, -0.7f));
        }

        if (other.CompareTag("LeaveHouse"))
        {
            MoveToPos(new Vector2(2.3f, 70f));
        }

        if (other.CompareTag("Leave"))
        {
            UIManager.instance.SetLeftVal(valueFinalCounter.GetValue());
            UIManager.instance.ShowMenu(1, 0);
            _controls.Disable();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Camera"))
        {
            _isExposed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Camera"))
        {
            _isExposed = false;
        }
    }
}
