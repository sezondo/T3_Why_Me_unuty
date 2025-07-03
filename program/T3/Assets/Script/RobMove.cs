using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class RobMove : MonoBehaviour
{
    private RobBase robBase;
    private NavMeshAgent agent;
    public Transform currentTarget;
    private bool isTargetRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        robBase = GetComponent<RobBase>();
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(Tracking());

        agent.speed = robBase.data.moveSpeed;
        isTargetRotation = false;

    }

    // Update is called once per frame
    void Update()
    {
        switch (robBase.currentState)
        {
            case UnitState.Idle:

                break;

            case UnitState.Attacking:
                //agent.stoppingDistance = robBase.data.attackIntersection;
                break;

            case UnitState.Moving:
                agent.stoppingDistance = 0f;

                break;

            case UnitState.Dead:

                break;

            case UnitState.Turn:
                agent.stoppingDistance = robBase.data.attackIntersection;
                break;
                
            case UnitState.Hurt:

                break;

        }
    }


    private IEnumerator Tracking()
    {
        while (true)
        {
            //여기는 네비게이션 매쉬 감지하는 쪽
            if (robBase.currentState == UnitState.Moving || robBase.currentState == UnitState.Idle)
            {
                TrackTarget();
            }
            else
            {
                StopMoving();
            }

            //여기는 레이케스트로 해서 상태변환시키는거(너무 길어서 함수로 변환)
            //TryAttackByRaycast();

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void TrackTarget()
    {
        agent.isStopped = false;

        
        currentTarget = FindNearestEnemyInRange();
        if (currentTarget != null)
        {
            
            if (robBase.currentState != UnitState.Moving)
                robBase.ChangeState(UnitState.Moving);

            agent.SetDestination(currentTarget.position);
        }
        else
        {
            StopMoving();
            robBase.ChangeState(UnitState.Idle);
        }
    }

/*
    private void TryAttackByRaycast()
    {
        if (currentTarget == null) return;

        Vector3 dir = (currentTarget.position - transform.position).normalized;
        float attackRange = robBase.data.attackIntersection;
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 1f, dir, out hit, attackRange))
        {
            RobBase enemy = hit.collider.GetComponent<RobBase>();
            if (enemy != null && enemy.data.faction != robBase.data.faction)
            {
                agent.stoppingDistance = robBase.data.attackIntersection;
                if (!isTargetRotation)
                {
                    isTargetRotation = true;
                    StartCoroutine(TargetRotation());
                }

                //robBase.ChangeState(UnitState.Attacking);
            }
            else
            {
                agent.stoppingDistance = 0f;
            }
        }
        DrawDebugRay(transform.position, dir, attackRange); // 이건 디버그용
    }
    */

    private void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    public Transform FindNearestEnemyInRange()//Moving면 1초 간격으로 추적 
    {
        RobBase[] enemies = FindObjectsByType<RobBase>(FindObjectsSortMode.None);
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (RobBase enemy in enemies)
        {
            if (enemy.data.faction == robBase.data.faction) continue;
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }
    public void TryStartRotation()
    {
        if (!isTargetRotation)
        {
            robBase.ChangeState(UnitState.Turn);
            StartCoroutine(TargetRotation());
        }
        
    }

    public IEnumerator TargetRotation()
    {
        while (true)
        {
            isTargetRotation = true;
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            direction.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * robBase.data.rotationSpeed);

            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            if (angle < robBase.data.RotationThreshold)
            {
                robBase.ChangeState(UnitState.Attacking);
                isTargetRotation = false;
                yield break;
            }


            yield return null;
        }
    }
    


}
