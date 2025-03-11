using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
public enum AIState
{
    Idle,
    Wandering,
    Attacking,
}

public class AIEnemy : MonoBehaviour
{
    [Header("Stats")]
    public float walkSpeed;
    public float runSpeed;

    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance;
    private AIState aiState;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Cambat")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    private float playerDistance;

    public float fieldOfView = 120f;

    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        SetState(AIState.Wandering);
    }

    private void Update()
    {
        playerDistance = Vector3.Distance(transform.position, CharacterManager.Instance.Player.transform.position);

        //  속도가 빠르면 뛰는 모션, 느리면 걷는 모션
        if(agent.speed > 2.0f)
        {
            animator.SetBool("IsRun", true);
            animator.SetBool("IsMove", false);
        }
        else
        {
            animator.SetBool("IsRun", false);
            animator.SetBool("IsMove", true);
        }

        // enum형 AIState의 값에 따라 상태 전환
        switch (aiState)
        {
            case AIState.Idle:
            case AIState.Wandering:
                PassiveUpdate();
                break;
            case AIState.Attacking:
                AttackingUpdate();
                break;

        }
    }

    //  Enemy의 상태를 전환
    public void SetState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed;
                agent.isStopped = true;
                break;
            case AIState.Wandering:
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Attacking:
                agent.speed = runSpeed;
                agent.isStopped = false;
                break;
        }
    }


    //  조건에 따라 Enemy의 상태를 전환
    private void PassiveUpdate()
    {
        if(aiState == AIState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        if(playerDistance < detectDistance)
        {
            SetState(AIState.Attacking);
        }
    }

    // 방황 할 때 새로운 목적지를 찾음
    private void WanderToNewLocation()
    {
        if (aiState != AIState.Idle)
        {
            return;
        }

        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    //  랜덤한 위치를 샘플링하는 함수
    private Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;

        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
            {
                break;
            }
        }

        return hit.position;
    }

    //  플레이어를 감지하고 공격하는 함수, 플레이어가 감지 범위 밖으로 나가면 다시 방황하도록 State변경
    private void AttackingUpdate()
    {
        if (playerDistance < attackDistance && isPlayerFreldOfView())
        {
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackRate)
            {
                lastAttackTime = Time.time;   
                animator.speed = 2;
                animator.SetTrigger("IsAttack");
            }
        }
        else
        {
            if (playerDistance < detectDistance)
            {
                agent.isStopped = false;
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(CharacterManager.Instance.Player.transform.position, path))
                {
                    agent.SetDestination(CharacterManager.Instance.Player.transform.position);
                }
                else
                {
                    agent.SetDestination(transform.position);
                    agent.isStopped = true;
                    SetState(AIState.Wandering);
                }
            }
            else
            {
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Wandering);
            }
        }
    }

    //  데미지를 계산해주는 함수. 공격 모션 animaion clip의 타이밍에 맞춰 event로 호출된다.
    private void CalcDamage()
    {
        CharacterManager.Instance.Player.controller.GetComponent<IDamagable>().Damage(damage);
    }

    //  Enemy의 시야각
    private bool isPlayerFreldOfView()
    {
        Vector3 directionToPlayer = CharacterManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        return angle < fieldOfView * 0.5f;
    }
}
