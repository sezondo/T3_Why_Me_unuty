using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class RobMove : MonoBehaviour
{
    private RobBase robBase;
    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        robBase = GetComponent<RobBase>();
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(Tracking());

        agent.speed = robBase.data.moveSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        switch (robBase.currentState)
        {
            case UnitState.Idle:

                break;

            case UnitState.Attacking:

                break;

            case UnitState.Moving:

                break;

            case UnitState.Dead:

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
            TryAttackByRaycast();

            yield return new WaitForSeconds(1f);
        }
    }

    private void TrackTarget()
    {
        agent.isStopped = false;

        Transform currentTarget;
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

    private void TryAttackByRaycast()
    {
        Vector3 dir = transform.forward;
        float attackRange = robBase.data.attackIntersection;
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 1f , dir, out hit, attackRange))
        {
            RobBase enemy = hit.collider.GetComponent<RobBase>();
            if (enemy != null && enemy.data.faction != robBase.data.faction)
            {
                robBase.ChangeState(UnitState.Attacking);
            }
        }
        DrawDebugRay(transform.position, dir, attackRange); // 이건 디버그용
    }

    private void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    Transform FindNearestEnemyInRange()//Moving면 1초 간격으로 추적 
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
    
    private void DrawDebugRay(Vector3 origin, Vector3 dir, float length)
    {
        Debug.DrawRay(origin + Vector3.up * 1f, dir * length, Color.red, 1f); // 1초 동안 씬 뷰에서 보임
    }


}
