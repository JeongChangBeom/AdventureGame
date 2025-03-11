using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LauncherPlatform : MonoBehaviour,IInteractable
{
    private PlayerInput playerInput;
    private Transform playerTransform;
    private Rigidbody playerRigidbody;

    [SerializeField] private float launchAngle;
    [SerializeField] private float launchForce;

    private void Start()
    {
        playerInput = CharacterManager.Instance.Player.GetComponent<PlayerInput>();
        playerTransform = CharacterManager.Instance.Player.transform;
        playerRigidbody = CharacterManager.Instance.Player.GetComponent<Rigidbody>();
    }

    public string GetInteractPrompt()
    {
        string str = "[고속 이동 장치]\n'E'키를 눌러 탑승";

        return str;
    }

    public void OnInteract()
    {
        LaunchReady();
    }

    //  발사 준비를 위해 못움직이도록 입력을 끊고, Player를 정해진 위치로 이동 시킴
    private void LaunchReady()
    {
        playerInput.actions["Move"].Disable();
        playerInput.actions["Jump"].Disable();
        playerTransform.position = transform.position + new Vector3(0f, 1f, 0f);
        playerTransform.rotation = transform.rotation * Quaternion.Euler(0, 180f, 0);

        Invoke("LaunchStart", 3f);
    }

    //  정해진 각도와 힘으로 Player를 발사
    private void LaunchStart()
    {
        CharacterManager.Instance.Player.controller.onLauncher = true;

        float angle = launchAngle * Mathf.Deg2Rad;
        Vector3 force = new Vector3(0, Mathf.Sin(angle), -Mathf.Cos(angle)) * launchForce;
        playerRigidbody.AddForce(force, ForceMode.Impulse);

        playerInput.actions["Move"].Enable();
        playerInput.actions["Jump"].Enable();
    }

}
