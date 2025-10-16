using UnityEngine;
using Unity.VisualScripting;

public class RobHpNet : RobHp
{
    private bool dieCheck;

    public override void Spawned()
    {
        dieCheck = false;

        if (Object.HasStateAuthority)
        {
            base.Start();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (currentHp <= 0 && robBase.currentState != UnitState.Dead)
            {
                robBase.ChangeState(UnitState.Dead);
            }
        }
        if (robBase.currentState == UnitState.Dead && !dieCheck)
        {
            dieCheck = true;
            DeadDisposal();
        }
    }
    

    public override void TakeDamage(int damage)
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }
        base.TakeDamage(damage);
    }

    public override void Destroy()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    public override void Start() { }
    public override void Update() { }
}
