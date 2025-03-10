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

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
        }

        ClearSelctedItemWindow();
        UpdateUI();
    }

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

        // ¿Â∫Ò√¢¿Ã ≤À√°¿ª ∂ß
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

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }
}
