using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Rob08AttackNet : RobAttackNet
{
    [HideInInspector] public Rob08StooterNet[] rob08Shooters;

    public override void Spawned()
    {
        base.Spawned();
        rob08Shooters = GetComponentsInChildren<Rob08StooterNet>(true);
        shooter = rob08Shooters;
    }

    public override IEnumerator Attacking()
    {
        var interval = Mathf.Max(0.05f, robBase.data.attackSpeed);
        while (Object && Object.IsValid && robBase.currentState == UnitState.Attacking)
        {
            Fire();
            yield return new WaitForSeconds(interval);
        }
    
        StopFire();
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
        if (!Object.HasStateAuthority)
            return;

        foreach (var st in rob08Shooters)
        {
            st.BeginFire(robBase.data.attackSpeed);
        }

        AttackTick++; 
    
    }

    private void StopFire()
    {
        if (!Object.HasStateAuthority)
        return;
        foreach (var st in rob08Shooters)
            st.StopFire();
    }
}
