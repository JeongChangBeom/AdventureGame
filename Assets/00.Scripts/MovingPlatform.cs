using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Vector3 moveTo = Vector3.zero;
    [SerializeField] float moveTime = 1f;

    Vector3 startPosition;

    private void Start()
    {
        Move();
    }

    private void Move()
    {
        transform.DOMove(transform.position + moveTo, moveTime).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.transform.SetParent(this.transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        collision.gameObject.transform.SetParent(null);
    }
}
