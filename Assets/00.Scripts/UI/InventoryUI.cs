using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;

    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedStatName;
    public TextMeshProUGUI selectedStatValue;
    public GameObject equipButton;
    public GameObject unequipButton;

    private PlayerController controller;
    private PlayerCondition condition;

    private ItemData selectedItem;
    private int selectedItemIndex = 0;

    int curEquipIndex;

    private void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;

        controller.inventory += Toggle;
        CharacterManager.Instance.Player.addItem += AddItem;

        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        //  인벤토리에 존재하는 slot들을 초기화
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }

        ClearSelctedItemWindow();
        UpdateUI();
    }

    //  인벤토리창 초기화
    private void ClearSelctedItemWindow()
    {
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        equipButton.SetActive(false);
        unequipButton.SetActive(false);
    }

    public void Toggle()
    {
        if (isOpen())
        {
            inventoryWindow.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            inventoryWindow.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public bool isOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    //  아이템을 획득하여 인벤토리 슬롯에 넣는 함수
    private void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = data;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        // 추후 장비창이 꽉찼을 때 로직 추가해야 됨. 현재는 장비창이 꽉찰수가 없음
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    //  슬롯들중 빈슬롯을 찾아주는 함수
    private ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    //  현재 선택한 슬롯의 정보를 출력
    public void SelectItem(int index)
    {
        if (slots[index].item == null)
        {
            return;
        }

        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == slots[index])
            {
                slots[i].selected = true;
            }
            else
            {
                slots[i].selected = false;
            }
        }

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = $"[{selectedItem.itemName}]";
        selectedItemDescription.text = selectedItem.description;

        selectedStatName.text = string.Empty;
        selectedStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.equipables.Length; i++)
        {
            selectedStatName.text += selectedItem.equipables[i].valueName + "\n";
            selectedStatValue.text += selectedItem.equipables[i].value > 0 ? $"+{selectedItem.equipables[i].value.ToString()}" + "\n" : "\n";
        }

        equipButton.SetActive(selectedItem.itemType == ItemType.Equipable && !slots[index].equiped);
        unequipButton.SetActive(selectedItem.itemType == ItemType.Equipable && slots[index].equiped);
    }

    //  장착 버튼을 누르면 아이템을 장착 + 아이템 장착 시 처리
    public void OnEquipButton()
    {
        if (slots[curEquipIndex].equiped)
        {
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].equiped = true;
        curEquipIndex = selectedItemIndex;
        controller.EquipItem(slots[selectedItemIndex].item);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    //  아이템 해제 시 처리
    void UnEquip(int index)
    {
        slots[index].equiped = false;
        controller.UnEquipItem(slots[index].item);
        UpdateUI();

        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    //  해제 버튼을 누르면 아이템 해제
    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }
}
