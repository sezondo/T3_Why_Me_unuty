using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/BulletData", fileName = "BulletData")]
public class BulletData : ScriptableObject
{
    public FactionType faction;
    public float bulletSpeed;
    public float bulletLifeTime;
    public int bulletDamage;
    public float arcHeight;
    public float explosionRadius;
    //public GameObject bulletPrefab;
}
