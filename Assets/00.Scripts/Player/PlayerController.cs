using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    public float maxJumpCount;
    public float curJumpCount;
    public bool isJump;
    public bool isDash;
    public float dashStamina;
    public float dashSpeed;
    private Vector3 _moveDirection;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float _camCurXRot;
    private float _camCurYRot;
    public float lookSensitivity;
    private Vector2 _mouseDelta;
    private float _camDistance;
    public float rotTime;
    public float rotSpeed;
    private Vector3 _targetRotaion;
    private Vector3 _curVelocity;

    public Action inventory;
    private Rigidbody _rigidbody;
    private Animator _anim;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isJump)
        {
            if (IsGrounded())
            {
                _anim.SetBool("IsJump", false);
                curJumpCount = 0;
                isJump = false;
            }
        }

        if (isDash)
        {
            CharacterManager.Instance.Player.condition.UseStamina(dashStamina * Time.deltaTime);

            if (CharacterManager.Instance.Player.condition.stamina.curValue <= 1f)
            {
                EndDash();
            }
        }

        CalcCamDistance();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    private void CalcCamDistance()
    {
        _camDistance = 4.0f + (moveSpeed * 0.2f);
    }

    private void Move()
    {
        Vector3 lookForward = new Vector3(cameraContainer.forward.x, 0f, cameraContainer.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraContainer.right.x, 0f, cameraContainer.right.z).normalized;

        Vector3 dir = lookForward * _moveDirection.y + lookRight * _moveDirection.x;

        if (dir.magnitude > 0.1f)
        {
            Quaternion viewRot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, viewRot, Time.deltaTime * rotSpeed);
        }

        dir *= moveSpeed;

        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
        _anim.SetBool("IsMove", _moveDirection.magnitude > 0.5f);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _moveDirection = Vector2.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curJumpCount < maxJumpCount)
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
            isJump = true;
            curJumpCount++;
            _anim.SetBool("IsJump", true);

            Invoke("EndJump", 0.1f);
        }
    }

    private void EndJump()
    {
        isJump = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && IsGrounded() && !isDash)
        {
            if (CharacterManager.Instance.Player.condition.stamina.curValue > 100f)
            {
                isDash = true;
                moveSpeed += dashSpeed;
            }
        }
        else if (context.phase == InputActionPhase.Canceled && isDash)
        {
            EndDash();
        }
    }

    private void EndDash()
    {
        isDash = false;
        moveSpeed -= dashSpeed;
    }

    private void CameraLook()
    {
        _camCurXRot += _mouseDelta.y * lookSensitivity;
        _camCurXRot = Mathf.Clamp(_camCurXRot, minXLook, maxXLook);

        _camCurYRot += _mouseDelta.x * lookSensitivity;

        _targetRotaion = Vector3.SmoothDamp(_targetRotaion, new Vector3(-_camCurXRot, _camCurYRot), ref _curVelocity, rotTime);
        cameraContainer.transform.eulerAngles = _targetRotaion;
        cameraContainer.transform.position = (transform.position - cameraContainer.forward * _camDistance) + Vector3.up;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    private void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position+(transform.forward*0.2f)+(transform.up*0.01f),Vector3.down),
            new Ray(transform.position+(-transform.forward*0.2f)+(transform.up*0.01f),Vector3.down),
            new Ray(transform.position+(transform.right*0.2f)+(transform.up*0.01f),Vector3.down),
            new Ray(transform.position+(-transform.right*0.2f)+(transform.up*0.01f),Vector3.down)
        };

        foreach (var ray in rays)
        {
            Debug.DrawRay(ray.origin, ray.direction * 0.1f, Color.red);

            if (Physics.Raycast(ray, 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void ConsumableItemEff()
    {
        if (CharacterManager.Instance.Player.itemData.effType == EffType.SpeedUp)
        {
            StartCoroutine(SpeedBooster());
        }
        else if (CharacterManager.Instance.Player.itemData.effType == EffType.JumpUp)
        {
            StartCoroutine(JumpBooster());
        }
    }

    private IEnumerator SpeedBooster()
    {
        float speedValue = CharacterManager.Instance.Player.itemData.value;

        moveSpeed += speedValue;

        yield return new WaitForSeconds(5.0f);

        moveSpeed -= speedValue;
    }

    private IEnumerator JumpBooster()
    {
        float jumpValue = CharacterManager.Instance.Player.itemData.value;

        jumpPower += jumpValue;

        yield return new WaitForSeconds(5.0f);

        jumpPower -= jumpValue;
    }

    public void EquipItem(ItemData item)
    {
        ItemData curItem = item;

        for (int i = 0; i < curItem.equipables.Length; i++)
        {
            switch (curItem.equipables[i].valueType)
            {
                case EquipableItemType.JumpCountUp:
                    maxJumpCount += curItem.equipables[i].value;
                    break;
                case EquipableItemType.JumpUp:
                    jumpPower += curItem.equipables[i].value;
                    break;
                case EquipableItemType.SpeedUp:
                    moveSpeed += curItem.equipables[i].value;
                    break;
            }
        }
    }
    public void UnEquipItem(ItemData item)
    {
        ItemData curItem = item;

        for (int i = 0; i < curItem.equipables.Length; i++)
        {
            switch (curItem.equipables[i].valueType)
            {
                case EquipableItemType.JumpCountUp:
                    maxJumpCount -= curItem.equipables[i].value;
                    break;
                case EquipableItemType.JumpUp:
                    jumpPower -= curItem.equipables[i].value;
                    break;
                case EquipableItemType.SpeedUp:
                    moveSpeed -= curItem.equipables[i].value;
                    break;
            }
        }
    }
}
