using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObejct : MonoBehaviour
{
    public float jumpPower;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rigidbody = collision.collider.GetComponent<Rigidbody>();

            if (rigidbody != null)
            {
                rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
        }
    }
}
