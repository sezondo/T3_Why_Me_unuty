using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable/RobData", fileName = "RobData")]

public class RobData : ScriptableObject
{
    public int maxHp;
    public int attackIntersection;
    public int moveSpeed;
    public int attackSpeed;
    public float rotationSpeed;
    public float RotationThreshold;
    public FactionType faction;
    public GameObject bulletPrefab;


}
