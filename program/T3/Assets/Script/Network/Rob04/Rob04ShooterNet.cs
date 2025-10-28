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

        Vector3 targetPos = robMove.currentTarget != null
            ? robMove.currentTarget.position
            : transform.position;

        var missile = Runner.Spawn(
        robBase.data.bulletPrefab,
        transform.position,
        transform.rotation);

        missile.GetComponent<Rob04BulletsNet>()
           .RPC_SetTarget(targetPos);
        /*
        NetworkObject missileNet;
        missileNet = Runner.Spawn(base.robBase.data.bulletPrefab, transform.position, transform.rotation);
        Rob04BulletsNet rob04Bullets = missileNet.GetComponent<Rob04BulletsNet>();
        rob04Bullets.TargetPoint(robMove.currentTarget);
        */
    }
}
