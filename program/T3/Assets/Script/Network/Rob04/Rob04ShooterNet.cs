using UnityEngine;
using Fusion;

public class Rob04ShooterNet : RobShooterNet
{
    private RobMove robMove;

    public override void Start()
    {
        base.Start();

        robMove = GetComponentInParent<RobMove>();
    }

    public override void Shoot()
    {
        if (!Object.HasStateAuthority)
            return;

        Vector3 spawnPos = transform.position;
        Vector3 targetPos = robMove.currentTarget != null
            ? robMove.currentTarget.position
            : spawnPos;

        var missile = Runner.Spawn(
        robBase.data.bulletPrefab,
        spawnPos);

        missile.GetComponent<Rob04BulletsNet>()
           .RPC_SetTarget(spawnPos, targetPos);
        /*
        NetworkObject missileNet;
        missileNet = Runner.Spawn(base.robBase.data.bulletPrefab, transform.position, transform.rotation);
        Rob04BulletsNet rob04Bullets = missileNet.GetComponent<Rob04BulletsNet>();
        rob04Bullets.TargetPoint(robMove.currentTarget);
        */
    }
}
