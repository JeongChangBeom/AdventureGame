using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract();
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;
    public float rotationSpeed = 80f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public string GetInteractPrompt()
    {
        string str = $"[{data.itemName}]";

        if (data.itemType == ItemType.Equipable)
        {
            str += "\n'E'키를 눌러 획득";
        }
        else if (data.itemType == ItemType.Consumable)
        {
            str += "\n'E'키를 눌러 사용";
        }

        return str;
    }

    public void OnInteract()
    {
        CharacterManager.Instance.Player.itemData = data;

        if (data.itemType == ItemType.Equipable)
        {
            CharacterManager.Instance.Player.addItem?.Invoke();
        }
        else if (data.itemType == ItemType.Consumable)
        {
            CharacterManager.Instance.Player.controller.ConsumableItemEff();
        }
        Destroy(gameObject);
    }
}
