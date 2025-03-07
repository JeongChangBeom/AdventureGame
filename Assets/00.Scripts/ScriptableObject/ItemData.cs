using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Equipable,
    Consumable,
}

public enum EffType
{
    Null,
    SpeedUp,
    JumpUp,
}

public enum EquipableItemType
{
    JumpCountUp,
    SpeedUp,
    JumpUp,
}

[Serializable]
public class ItemDataEquipable
{
    public EquipableItemType valueType;
    public string valueName;
    public float value;
}


[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    public string description;
    public ItemType itemType;

    [Header("Equipable")]
    public Sprite icon;
    public ItemDataEquipable[] equipables;

    [Header("Consumable")]
    public EffType effType;
    public float effDuration;
    public float value;
}
