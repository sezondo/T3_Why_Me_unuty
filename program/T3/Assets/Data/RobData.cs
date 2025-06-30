using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/RobData", fileName = "RobData")]

public class RobData : ScriptableObject
{
    public int maxHp;
    public int attackPower;
    public int attackIntersection;
    public int moveSpeed;
    public int attackSpeed;
    public FactionType faction;
}
