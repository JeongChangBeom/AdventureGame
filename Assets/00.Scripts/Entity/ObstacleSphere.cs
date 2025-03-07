using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSphere : MonoBehaviour
{
    public float damage;
    public float destroyTime;

    private void Start()
    {
        Invoke("DestroyObject", destroyTime);
    }

    private void DestroyObject()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CharacterManager.Instance.Player.condition.Damage(damage);
            DestroyObject();
        }
    }
}
