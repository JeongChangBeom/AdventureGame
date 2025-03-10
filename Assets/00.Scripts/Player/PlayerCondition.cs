using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void Damage(float damage);
}

public class PlayerCondition : MonoBehaviour, IDamagable
{
    public UIManager uiManager;

    public Condition health
    {
        get => uiManager.health;
    }

    public Condition stamina
    {
        get => uiManager.stamina;
    }

    private bool isDie;

    private void Start()
    {
        isDie = false;
    }

    private void Update()
    {
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (health.curValue <= 0f && !isDie)
        { 
            Die();
            isDie = true;
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Damage(float damage)
    {
        CharacterManager.Instance.Player.anim.SetTrigger("IsDamage");
        health.Subtract(damage);
    }

    public void Die()
    {
        CharacterManager.Instance.Player.anim.SetTrigger("IsDie");
        Invoke("DieGameOver",2f);
    }

    private void DieGameOver()
    {
        GameManager.Instance.GameOver();
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }
        stamina.Subtract(amount);
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            Die();
        }
    }
}
