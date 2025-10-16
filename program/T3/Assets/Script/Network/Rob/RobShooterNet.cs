using UnityEngine;
using System.Collections;

public class RobShooterNet : RobShooter
{

    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            StopAllCoroutines();
            this.enabled = false;
            return;
        }
        base.Start();
    }
    
    public override void Start(){ }

    public override void Shoot()
    {
        Runner.Spawn(robBase.data.bulletPrefab, transform.position, transform.rotation);
    }
}
