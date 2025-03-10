using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LaserTrap : MonoBehaviour
{
    [SerializeField] private GameObject obstacle;
    [SerializeField] private Transform target;
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask playerLayerMask;

    private bool isFirst = true;


    void Update()
    {
        if (isFirst)
        {
            RayCheck();
        }
    }

    void RayCheck()
    {
        Ray[] rays = new Ray[5]{
            new Ray(transform.position, transform.forward),
            new Ray(transform.position+transform.up, transform.forward),
            new Ray(transform.position+transform.up*2f, transform.forward),
            new Ray(transform.position+transform.up*3f, transform.forward),
            new Ray(transform.position+transform.up*4f, transform.forward)
        };

        foreach(var ray in rays)
        {
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);
            if (Physics.Raycast(ray, rayDistance, playerLayerMask))
            {
                SpawnObstacle();
                isFirst = false;
            }
        }
    }

    void SpawnObstacle()
    {
        Instantiate(obstacle, target.position, Quaternion.identity);
    }
}
