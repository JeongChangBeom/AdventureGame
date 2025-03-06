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
   SpeedUp,
   JumpUp,
}


[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public Sprite icon;
    public string itemName;
    public string description;
    public float value;
    public ItemType itemType;
    public EffType effType;
    public float effDuration;
}
