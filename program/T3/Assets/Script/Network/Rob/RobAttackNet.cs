using UnityEngine;
using System.Collections;

public class RobAttackNet : RobAttack
{

    public override void Spawned()
    {
        base.Start();
    }
    public override void Start() { }
    public override void Update(){}
    public override void FixedUpdateNetwork()
    {
        if (robBase.currentState == UnitState.Dead)
        {
            return;
        }

        switch (robBase.currentState)
        {
            case UnitState.Idle:

                break;

            case UnitState.Attacking:

                if (!CoroutineCheck)
                {
                    CoroutineCheck = true;
                    StartCoroutine(Attacking());
                }
                break;

            case UnitState.Moving:

                break;

            case UnitState.Dead:

                break;

            case UnitState.Turn:

                break;

        }
    }

   

    public override void Fire()
    {
        SoundManager.instance.PlaySFX(robBase.data.attackAudioClip, this.transform);

        if (Object.HasStateAuthority)
        {
            foreach (var fp in shooter)
            {
                fp.Shoot();

            }
        }
    }
    
}
