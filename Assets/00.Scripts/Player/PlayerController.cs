using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
    public float jumpStamina;
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

    [Header("Climb")]
    public bool isClimb;
    public float climbStamina;
    public float climbCount;

    [Header("")]
    public bool onLauncher;
    public Action inventory;
    private Rigidbody _rigidbody;
    private Animator _anim;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
    }

    private void Start()
    {
        isClimb = false;
        onLauncher = false;
    }

    private void Update()
    {
        //  땅에 닿았을 때 점프 횟수, 등반 횟수 초기화
        if (!isJump)
        {
            if (IsGrounded())
            {
                _anim.SetBool("IsJump", false);
                curJumpCount = 0;
                isJump = false;

                climbCount = 0;
            }
        }

        //  고속 이동 장치에 의해 날라가다가 땅에 닿았을 때
        if (onLauncher)
        {
            if (IsGrounded())
            {
                onLauncher = false;
            }
        }

        //  등반시 스태미너 사용, 스태미너가 없으면 등반 종료
        if (isClimb)
        {
            CharacterManager.Instance.Player.condition.UseStamina(climbStamina * Time.deltaTime);

            if (CharacterManager.Instance.Player.condition.stamina.curValue <= 1f)
            {
                EndClimb();
            }
        }

        //  대쉬시 스태미너 사용, 스태미너가 없으면 대쉬 종료
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
        //  고속이동 장치에 의해 날라가고 있지 않거나, 등반중이지 않으면 Move()호출 
        if (!onLauncher && !isClimb)
        {
            Move();
        }
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    //  속도에 따른 카메라 거리 계산
    private void CalcCamDistance()
    {
        _camDistance = 4.0f + (moveSpeed * 0.2f);
    }

    //  카메라가 바라보는 방향에 따른 이동
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

    //  Input System을 통한 키입력으로 이동할 방향을 받아옴
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

    //  Input System을 통한 키입력으로 점프를 진행
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curJumpCount < maxJumpCount)
        {
            if(CharacterManager.Instance.Player.condition.stamina.curValue > jumpStamina)
            {
                EndClimb();
                CharacterManager.Instance.Player.condition.UseStamina(jumpStamina);

                _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                isJump = true;
                curJumpCount++;
                _anim.SetBool("IsJump", true);

                Invoke("EndJump", 0.1f);
            }
        }
    }

    private void EndJump()
    {
        isJump = false;
    }

    //  Input System을 통한 키입력으로 대쉬를 진행
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

    // 3인칭 카메라 시점
    private void CameraLook()
    {
        _camCurXRot += _mouseDelta.y * lookSensitivity;
        _camCurXRot = Mathf.Clamp(_camCurXRot, minXLook, maxXLook);

        _camCurYRot += _mouseDelta.x * lookSensitivity;

        _targetRotaion = Vector3.SmoothDamp(_targetRotaion, new Vector3(-_camCurXRot, _camCurYRot), ref _curVelocity, rotTime);
        cameraContainer.transform.eulerAngles = _targetRotaion;
        cameraContainer.transform.position = (transform.position - cameraContainer.forward * _camDistance) + Vector3.up;
    }

    //  Input System을 통해 마우스의 delta값을 받아옴
    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    //  Input System을 통해 특정 키를 누르면 인벤토리 활성화
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    //  Input System을 통해 특정 키를 누르면 벽에 매달림
    public void OnClimb(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            Ray ray = new Ray(transform.position + transform.up * 1.5f, transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 1f, Color.red);

            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 1f))
            {
                if (hit.normal.y < 0.1f && climbCount < 1)
                {
                    if (!IsGrounded())
                    {
                        //  이부분에 원래 벽에 매달리는 애니메이션이 들어가야되는데 에셋에 매달리는 모션이 없음. 추후 수정 필요
                        _anim.SetBool("IsMove", false);
                        _anim.SetBool("IsJump", false);
                        //

                        isClimb = true;
                        climbCount++;

                        _rigidbody.drag = Mathf.Infinity;

                        curJumpCount = 0;
                        isJump = false;
                    }
                }
            }
        }
    }

    private void EndClimb()
    {
        _rigidbody.drag = 0;
        isClimb = false;
    }

    //  마우스 커서의 상태 전환
    private void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    //  Player가 지면에 닿았는 지 판단하는 함수
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

    //  소비아이템을 사용하는 함수
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

    //  소비아이템이 이동속도 부스터일 때
    private IEnumerator SpeedBooster()
    {
        float speedValue = CharacterManager.Instance.Player.itemData.value;

        moveSpeed += speedValue;

        yield return new WaitForSeconds(5.0f);

        moveSpeed -= speedValue;
    }

    //  소비아이템이 점프 부스터일 때
    private IEnumerator JumpBooster()
    {
        float jumpValue = CharacterManager.Instance.Player.itemData.value;

        jumpPower += jumpValue;

        yield return new WaitForSeconds(5.0f);

        jumpPower -= jumpValue;
    }

    //  아이템 장착 시 아이템 효과 적용
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

    //  아이템 해제 시 아이템 효과 미적용
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
