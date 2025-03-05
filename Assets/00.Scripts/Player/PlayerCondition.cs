using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UIManager uiManager;

    public Condition health
    {
        get => uiManager.health;
    }

    private void Update()
    {
        health.Add(health.passiveValue * Time.deltaTime);

        if (health.curValue <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Die()
    {
        // Á×À½
    }
}
