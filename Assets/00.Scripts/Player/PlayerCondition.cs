using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        //  자동으로 스태미너 회복
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        //  체력이 0이되면 죽음
        if (health.curValue <= 0f && !isDie)
        { 
            Die();
            isDie = true;
        }
    }

    //  체력회복
    public void Heal(float amount)
    {
        health.Add(amount);
    }

    //  데미지
    public void Damage(float damage)
    {
        CharacterManager.Instance.Player.anim.SetTrigger("IsDamage");
        health.Subtract(damage);
    }

    //  죽었을 때 애니메이션 호출, 일정시간이후 게임 오버
    public void Die()
    {
        CharacterManager.Instance.Player.GetComponent<PlayerInput>().enabled = false;
        CharacterManager.Instance.Player.anim.SetTrigger("IsDie");
        Invoke("DieGameOver",2f);
    }

    private void DieGameOver()
    {
        CharacterManager.Instance.Player.GetComponent<PlayerInput>().enabled = true;
        GameManager.Instance.GameOver();
    }

    //  스태미너 사용
    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }
        stamina.Subtract(amount);
        return true;
    }

    //  떨어졌을 때 사망
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            Die();
        }
    }
}
