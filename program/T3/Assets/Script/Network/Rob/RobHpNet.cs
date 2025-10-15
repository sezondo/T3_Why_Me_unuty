using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class RobHpNet : RobHp
{
    private bool IsServer => robBaseNetCash != null && robBaseNetCash.Object.HasStateAuthority;
    private RobBaseNet robBaseNetCash;
    private bool dieCheck;

    public void Awake()
    {
        robBaseNetCash = GetComponent<RobBaseNet>();
        dieCheck = false;
    }

    public override void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitUntil(() => robBaseNetCash.Object != null);

        if (!IsServer)
        {
            this.enabled = false;
            yield break;
        }


        base.Start();
    }

    public override void Update()
    {
        if (IsServer)
        {
            if (currentHp <= 0 && robBase.currentState != UnitState.Dead)
            {
                robBase.ChangeState(UnitState.Dead);
            }
        }

        if (robBaseNetCash.currentState == UnitState.Dead && dieCheck == false)
        {
            dieCheck = true;
            DeadDisposal();
        }
    }

    public override void TakeDamage(int damage)
    {
        if (!IsServer)
        {
            return;
        }
        base.TakeDamage(damage);
    }

    public override void Destroy()
    {
        if (IsServer)
        {
            Runner.Despawn(Object);
        }
    }

    
}
