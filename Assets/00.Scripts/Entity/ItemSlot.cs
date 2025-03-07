using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;

    public Button button;
    public Image icon;
    public GameObject equipStateText;
    private Outline outline;

    public InventoryUI inventory;

    public int index;
    public bool selected;
    public bool equiped;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void Update()
    {
        outline.enabled = selected;
    }

    private void OnEnable()
    {
        if (equiped)
        {
            equipStateText.SetActive(true);
        }
        else
        {
            equipStateText.SetActive(false);
        }
    }

    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;

        if (equiped)
        {
            equipStateText.SetActive(true);
        }
        else
        {
            equipStateText.SetActive(false);
        }
    }

    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        equipStateText.SetActive(false);
    }

    public void OnClickButton()
    {
        inventory.SelectItem(index);
    }
}
