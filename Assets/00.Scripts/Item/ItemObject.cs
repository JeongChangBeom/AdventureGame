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

    public string GetInteractPrompt()
    {
        string str = $"[{data.itemName}]\n{data.description}";

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
