using UnityEngine;
using System.Collections;


public class RobDetector : MonoBehaviour
{

    [HideInInspector]public RobBase robBase;
    [HideInInspector]public RobMove robMove;
    private Transform currentTarget => robMove.currentTarget;
    [HideInInspector]public bool isDetecting;
    [HideInInspector]public int layerMask;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        robBase = GetComponent<RobBase>();
        robMove = GetComponent<RobMove>();


        if (robBase.data.faction == FactionType.Ally)
        {
            layerMask = LayerMask.GetMask("Enemy", "Wall");
        }
        else if (robBase.data.faction == FactionType.Enemy)
        {
            layerMask = LayerMask.GetMask("Ally", "Wall");
        }

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

        }
    }

    public IEnumerator Detector()
    {
        while (true)
        {
            if (robBase.currentState == UnitState.Dead)
            {
                break;
            }

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

        if (Physics.Raycast(transform.position + Vector3.up * 1f, dir, out hit, attackRange, layerMask))
        {
#if UNITY_EDITOR
            Debug.Log($"[{name}] Raycast: HIT {hit.collider.name}");// 디버그용 로그
#endif


            RobBase enemy = hit.collider.GetComponent<RobBase>();
            if (enemy != null && enemy.data.faction != robBase.data.faction)
            {
                isDetecting = true;
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
#if UNITY_EDITOR
        DrawDebugRay(transform.position, dir, attackRange); // scene창에서 확인용
#endif
    }

    private void MissingTarget()
    {
        if (robBase.currentState == UnitState.Attacking)
            robBase.ChangeState(UnitState.Idle);

        isDetecting = false;
    }

    private void DrawDebugRay(Vector3 origin, Vector3 dir, float length)
    {
        Debug.DrawRay(origin + Vector3.up * 1f, dir * length, Color.red, 0.2f); // 0.2초 동안 씬 뷰에서 보임
    }
}
