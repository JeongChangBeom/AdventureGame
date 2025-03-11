using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  상호작용 가능한 오브젝트에 상속할 인터페이스
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

    //  Prompt창에 띄울 텍스트
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

    //  상호작용 했을 때 호출되는 함수
    public void OnInteract()
    {
        CharacterManager.Instance.Player.itemData = data;

        //  장비 아이템이면 인벤토리에 넣고, 소비 아이템이면 즉시 사용한다.
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
