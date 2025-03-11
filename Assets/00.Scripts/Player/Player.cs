using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Animations;

public class Player : MonoBehaviour
{
    public PlayerController controller;
    public PlayerCondition condition;
    public Animator anim;

    public ItemData itemData;
    public Action addItem;

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        anim = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
    }
}
