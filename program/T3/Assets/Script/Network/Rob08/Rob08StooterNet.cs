// Assets/Script/Network/Rob08/Rob08StooterNet.cs
public class Rob08StooterNet : RobShooterNet
{
    private Rob08BulletsNet rob08Bullets;

    public override void Spawned()
    {
        robBase = GetComponentInParent<RobBase>();
        rob08Bullets = GetComponentInChildren<Rob08BulletsNet>(true);
    }

    public override void Shoot()
    {
        if (rob08Bullets != null)
        {
            rob08Bullets.attackTime = robBase.data.attackSpeed;
            rob08Bullets.gameObject.SetActive(true);
        }
    }

    public void StootStop()
    {
        StartCoroutine(rob08Bullets.stopFire());
    }

    
}
