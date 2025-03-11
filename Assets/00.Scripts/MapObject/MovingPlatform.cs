using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Vector3 moveTo = Vector3.zero;
    [SerializeField] float moveTime = 1f;

    private void Start()
    {
        Move();
    }

    //  발판을 좌우로 움직이도록 하는 함수
    private void Move()
    {
        transform.DOMove(transform.position + moveTo, moveTime).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    }

    //  Player가 발판의 자식객체가 되어 발판위에서의 이동을 자연스럽게 만듦
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
