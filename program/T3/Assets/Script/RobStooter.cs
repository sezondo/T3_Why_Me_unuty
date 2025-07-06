using UnityEngine;

public class RobStooter : MonoBehaviour
{
    private RobBase robBase;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        robBase = GetComponentInParent<RobBase>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Stoot()
    {
        Instantiate(robBase.data.bulletPrefab, transform.position, transform.rotation);
    }
}
