using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Rob08AttackNet : RobAttackNet
{
    [HideInInspector] public Rob08StooterNet[] Rob08StooterNet;

    public override void Spawned()
    {
        animator = GetComponent<Animator>();
        robBase = GetComponent<RobBase>();
        Rob08StooterNet = GetComponentsInChildren<Rob08StooterNet>();

        shooter = Rob08StooterNet; // 상위 Fire 루프와 Tick 동기화 유지
    }

    public override IEnumerator Attacking()
    {
        while (true)
        {
            if (robBase.currentState == UnitState.Dead)
            {
                FireStop();
                break;
            }

            if (robBase.currentState != UnitState.Attacking)
            {
                FireStop();
                break;
            }

            Fire();

            yield return null;
        }
        CoroutineCheck = false;
    }

    public override void Render()
    {
        if (robBase.currentState == UnitState.Attacking)
        {
            animator.SetTrigger("Attack");
        }
    }

    public override void Fire()
    {
        foreach (var fp in Rob08StooterNet)
        {
            fp.Shoot();
        }
    }

    private void FireStop()
    {
        foreach (var fp in Rob08StooterNet)
        {
            fp.StootStop();
        }
    }
}
