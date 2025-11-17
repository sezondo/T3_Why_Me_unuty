using System.Collections;
using UnityEngine;

public class RobBossAttack : RobAttackNet
{
    public override void Render()
    {
        if (AttackTick > _lastAttackTick)
        {
            // 공격 애니메이션과 사운드를 재생합니다.
            animator.SetTrigger("Attack");
            
            // 로컬 Tick 값을 업데이트하여 다음 프레임에서 중복 실행되는 것을 방지합니다.
            _lastAttackTick = AttackTick;
        }
    }

    public void BossAttack() // 애니메이션 이벤트로 동작
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX(robBase.data.attackAudioClip, this.transform);
        }
        Fire();
        
    }
    public override IEnumerator Attacking()
    {
        while (true)
        {
            if (robBase.currentState == UnitState.Dead || robBase.currentState != UnitState.Attacking)
            {
                break;
            }

            //Fire();

            AttackTick++;

            yield return new WaitForSeconds(robBase.data.attackSpeed);
        }
        CoroutineCheck = false;
    }

    public override void Fire()
    {
        // 서버 권한이 있을 때만 발사 로직과 Tick 값 증가를 실행합니다.
        if (Object.HasStateAuthority)
        {
            // 1. 실제 총알 발사
            foreach (var fp in shooter)
            {
                fp.Shoot();
            }
            
        }
    }
}
