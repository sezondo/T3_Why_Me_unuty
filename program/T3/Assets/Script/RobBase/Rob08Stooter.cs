using Unity.VisualScripting;
using UnityEngine;

public class Rob08Stooter : RobStooter
{
    private Rob08Bullets rob08Bullets;
    void Awake()
    {
        rob08Bullets = GetComponentInChildren<Rob08Bullets>(true);
    }
    void Start()
    {
        base.Start();
        
    }
    public override void Stoot()
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
