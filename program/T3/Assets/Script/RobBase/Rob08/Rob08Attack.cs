using System.Collections;
using UnityEngine;

public class Rob08Attack : RobAttack
{
    [HideInInspector] public Rob08Stooter[] rob08Shooter;
    public override void Start()
    {
        animator = GetComponent<Animator>();
        robBase = GetComponent<RobBase>();
        rob08Shooter = GetComponentsInChildren<Rob08Stooter>();
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

            animator.SetTrigger("Attack");

            yield return null;
        }
        CoroutineCheck = false;
    }
    public override void Fire()
    {
        foreach (var fp in rob08Shooter)
        {
            fp.Stoot();
        }
    }
    private void FireStop()
    {
        foreach (var fp in rob08Shooter)
        {
            fp.StootStop();
        }
    }
}
