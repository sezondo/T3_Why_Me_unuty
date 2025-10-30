using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Rob08StooterNet : RobShooterNet
{
    [SerializeField] private Rob08BulletsNet rob08Bullets;

    public override void Spawned()
    {
         base.Spawned();     // robBase 세팅 + 프록시 비활성 처리
        if (!Object.HasStateAuthority)
            return;

        if (rob08Bullets == null)
            rob08Bullets = GetComponentInChildren<Rob08BulletsNet>(true);

        
    }

    public override void Shoot()
    {
        BeginFire(robBase.data.attackSpeed);
    }

    public void BeginFire(float interval) {
        if (!Object.HasStateAuthority || rob08Bullets == null)
            return;

        if (!rob08Bullets.gameObject.activeSelf)
            rob08Bullets.gameObject.SetActive(true);

        rob08Bullets.RPC_SetFire(true, interval);
    }

    public void StopFire() {
        if (!Object.HasStateAuthority || rob08Bullets == null)
            return;

        rob08Bullets.RPC_SetFire(false, robBase.data.attackSpeed);
    }

    
}
