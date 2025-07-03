using UnityEngine;
using System.Collections;


public class RobDetector : MonoBehaviour
{

    private RobBase robBase;
    private RobMove robMove;
    private Transform currentTarget => robMove.currentTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        robBase = GetComponent<RobBase>();
        robMove = GetComponent<RobMove>();
        StartCoroutine(Detector());        
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
            case UnitState.Turn:

                break;
            case UnitState.Hurt:

                break;

        }
    }

    private IEnumerator Detector()
    {
        while (true)
        {
            TryAttackByRaycast();

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void TryAttackByRaycast()
    {
        if (currentTarget == null) return;

        Vector3 dir = (currentTarget.position - transform.position).normalized;
        float attackRange = robBase.data.attackIntersection;
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 1f, dir, out hit, attackRange))
        {
            Debug.Log($"[{name}] Raycast: HIT {hit.collider.name}");//로그지옥이당


            RobBase enemy = hit.collider.GetComponent<RobBase>();
            if (enemy != null && enemy.data.faction != robBase.data.faction)
            {
                robMove.TryStartRotation();
            }
            else if (enemy == null || enemy.data.faction == robBase.data.faction)
            {
                MissingTarget();
            }
        }
        else
        {
            MissingTarget();
        }
        DrawDebugRay(transform.position, dir, attackRange); // 이건 디버그용
    }

    private void MissingTarget()
    {
        if (robBase.currentState == UnitState.Attacking)
            robBase.ChangeState(UnitState.Idle);
    }
    
    private void DrawDebugRay(Vector3 origin, Vector3 dir, float length)
    {
        Debug.DrawRay(origin + Vector3.up * 1f, dir * length, Color.red, 0.2f); // 0.2초 동안 씬 뷰에서 보임
    }
}
