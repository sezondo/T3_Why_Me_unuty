using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/BulletData", fileName = "BulletData")]
public class BulletData : ScriptableObject
{
    public FactionType faction;
    public float bulletSpeed;
    public float bulletLifeTime;
    public int bulletDamage;
    //public GameObject bulletPrefab;
}
