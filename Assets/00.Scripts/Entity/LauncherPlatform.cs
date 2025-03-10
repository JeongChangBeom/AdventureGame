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

    private void LaunchReady()
    {
        playerInput.actions["Move"].Disable();
        playerInput.actions["Jump"].Disable();
        playerTransform.position = transform.position + new Vector3(0f, 1f, 0f);
        playerTransform.rotation = transform.rotation * Quaternion.Euler(0, 180f, 0);

        Invoke("LaunchStart", 3f);
    }

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
