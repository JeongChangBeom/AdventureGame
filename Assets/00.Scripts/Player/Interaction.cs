using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Interaction : MonoBehaviour
{
    public float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractGameObject;
    private IInteractable curInteractable;

    public TextMeshProUGUI promptText;

    private void Update()
    {
        Ray[] rays = new Ray[12]
        {
            new Ray(transform.position, transform.forward),
            new Ray(transform.position + transform.up*0.5f, transform.forward),
            new Ray(transform.position + transform.up*1f, transform.forward),
            new Ray(transform.position + transform.up*1.5f, transform.forward),

            new Ray(transform.position + transform.right*0.3f, transform.forward),
            new Ray(transform.position + transform.up*0.5f + transform.right*0.3f, transform.forward),
            new Ray(transform.position + transform.up*1f + transform.right*0.3f, transform.forward),
            new Ray(transform.position + transform.up*1.5f + transform.right*0.3f, transform.forward),

            new Ray(transform.position - transform.right*0.3f, transform.forward),
            new Ray(transform.position + transform.up*0.5f - transform.right*0.3f, transform.forward),
            new Ray(transform.position + transform.up*1f - transform.right*0.3f, transform.forward),
            new Ray(transform.position + transform.up*1.5f - transform.right*0.3f, transform.forward)
        };

        int noHitCheck = 0;

        foreach (var ray in rays)
        {
            Debug.DrawRay(ray.origin, ray.direction * maxCheckDistance, Color.red);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }
            else
            {
                noHitCheck++;
            }
        }

        if(noHitCheck == rays.Length)
        {
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }


    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
}
