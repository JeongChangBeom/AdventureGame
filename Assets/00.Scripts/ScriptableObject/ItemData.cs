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


[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    public string description;
    public float value;
    public ItemType itemType;

    [Header("Equipable")]
    public Sprite icon;

    [Header("Consumable")]
    public EffType effType;
    public float effDuration;
}
