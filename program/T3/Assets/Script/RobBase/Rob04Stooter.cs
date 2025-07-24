using UnityEngine;

public class Rob04Stooter : RobStooter
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private RobMove robMove;

    public override void Start()
    {
        base.Start();

        robMove = GetComponentInParent<RobMove>();
    }

    public override void Stoot()
    {
        GameObject missile;
        missile = Instantiate(base.robBase.data.bulletPrefab, transform.position, transform.rotation);
        Rob04Bullets rob04Bullets = missile.GetComponent<Rob04Bullets>();
        rob04Bullets.TargetPoint(robMove.currentTarget);
    }
}
