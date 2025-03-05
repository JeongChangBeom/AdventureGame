using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    public bool isDash;
    public float dashValue;
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
    public bool canLook = true;
    public float camDistance;
    public float rotTime;
    public float rotSpeed;
    private Vector3 _targetRotaion;
    private Vector3 _curVelocity;

    private Rigidbody _rigidbody;
    private Animator _anim;

    private float _originMoveSpeed;
    private float _originCamDistance;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _originMoveSpeed = moveSpeed;
        _originCamDistance = camDistance;
    }

    private void Update()
    {
        if (IsGrounded())
        {
            _anim.SetBool("IsJump", false);
        }

        if (isDash)
        {
            CharacterManager.Instance.Player.condition.UseStamina(dashValue * Time.deltaTime);

            if (CharacterManager.Instance.Player.condition.stamina.curValue <= 40f)
            {
                EndDash();
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
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

        if (!IsGrounded())
        {
            dir *= moveSpeed / 2.0f;
        }
        else
        {
            dir *= moveSpeed;
        }

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
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }

        _anim.SetBool("IsJump", true);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && IsGrounded())
        {
            if(CharacterManager.Instance.Player.condition.stamina.curValue > 40f)
            {
                isDash = true;
                moveSpeed *= 2.0f;
                camDistance *= 1.2f;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            EndDash();
        }
    }

    private void EndDash()
    {
        isDash = false;
        moveSpeed = _originMoveSpeed;
        camDistance = _originCamDistance;
    }

    bool IsGrounded()
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
            if (Physics.Raycast(ray, 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    private void CameraLook()
    {
        _camCurXRot += _mouseDelta.y * lookSensitivity;
        _camCurXRot = Mathf.Clamp(_camCurXRot, minXLook, maxXLook);

        _camCurYRot += _mouseDelta.x * lookSensitivity;

        _targetRotaion = Vector3.SmoothDamp(_targetRotaion, new Vector3(-_camCurXRot, _camCurYRot), ref _curVelocity, rotTime);
        cameraContainer.transform.eulerAngles = _targetRotaion;
        cameraContainer.transform.position = (transform.position - cameraContainer.forward * camDistance) + Vector3.up;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }
}
