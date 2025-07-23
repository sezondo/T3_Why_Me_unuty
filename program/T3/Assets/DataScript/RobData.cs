using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable/RobData", fileName = "RobData")]

public class RobData : ScriptableObject
{
    public int maxHp;
    public float attackIntersection;
    public float moveSpeed;
    public float attackSpeed;
    public float rotationSpeed;
    public float RotationThreshold;
    public FactionType faction;
    public GameObject bulletPrefab;


}
