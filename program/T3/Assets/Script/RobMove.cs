using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class RobMove : MonoBehaviour
{
    private RobBase robBase;
    private RobDetector robDetector;
    private NavMeshAgent agent;
    public Transform currentTarget;
    private bool isTargetRotation;
    private Coroutine rotationCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        robBase = GetComponent<RobBase>();
        robDetector = GetComponent<RobDetector>();
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
            //if (robBase.currentState == UnitState.Moving || robBase.currentState == UnitState.Idle)
            if (robBase.currentState != UnitState.Dead)
            {
                TrackTarget();
            }
            else
            {
                StopMoving();
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void TrackTarget()
    {
        agent.isStopped = false;

        //애가 현재 추적 좌표임
        currentTarget = FindNearestEnemyInRange();
        if (currentTarget != null)
        {
            setTarget();
        }
        else
        {
            StopMoving();
            robBase.ChangeState(UnitState.Idle);
        }
    }

    private void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    public Transform FindNearestEnemyInRange()//Moving면 1초 간격으로 추적 
    {
        //이건 가장 가까운놈 찾는거 이것도 항상 돌아가서 추적함
        RobBase[] enemies = FindObjectsByType<RobBase>(FindObjectsSortMode.None);
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (RobBase enemy in enemies)
        {
            if (enemy.data.faction == robBase.data.faction || enemy.currentState == UnitState.Dead) continue;
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }
        //여기서 넘겨주는놈이 현재 타겟 포지션인데 이 타겟 포지션을 Detector에서 받아가지고 레이케스트 쏴서 판별
        //레이케스트에서 리턴값이 적군이면 TargetRotation 실행하게 했음
        //즉 추적은 계속 하는데 판단은 RobDetector에서 함
        return nearest;
    }
    public void TryStartRotation()
    {
        if (rotationCoroutine != null)
        StopCoroutine(rotationCoroutine);

        robBase.ChangeState(UnitState.Turn);
        rotationCoroutine = StartCoroutine(TargetRotation());
    }

    public IEnumerator TargetRotation()
    {
        while (true)
        {
            //이건 네비게이트와 무관하게 아군을 로테이트 시키는 함수임
            //어차피 발동조건이 레이케스트 걸렸을때라가지고 네비매쉬랑 안겹침
            //위에서 상태에 따라서 내비매쉬 파라미터인 정지 거리를 늘렸다 주렸다함
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

    private void setTarget() 
    {
        // 이동 결정은 RobDetector에서 받음 해당 플래그는 적감지에 걸렸나 안걸렸나 확인하는 비트 즉 감지와 추적을 분리했음
        if (robDetector.isDetecting)
        {
            StopMoving();
        }
        else
        {
            agent.SetDestination(currentTarget.position);

            if (robBase.currentState != UnitState.Moving)
                robBase.ChangeState(UnitState.Moving);
        }
        
    }
    


}
