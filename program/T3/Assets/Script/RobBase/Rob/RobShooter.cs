using UnityEngine;

public class RobShooter : MonoBehaviour, IShooter
{
    public RobBase robBase;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        robBase = GetComponentInParent<RobBase>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Shoot()
    {
        Instantiate(robBase.data.bulletPrefab, transform.position, transform.rotation);
    }
}
