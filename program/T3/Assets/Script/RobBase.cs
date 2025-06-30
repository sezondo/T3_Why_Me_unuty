using UnityEngine;

public class RobBase : MonoBehaviour
{
    public RobData data;
    public UnitState currentState;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        currentState = UnitState.Idle;
    }

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            case UnitState.Idle:

                break;
            case UnitState.Moving:

                break;
            case UnitState.Attacking:

                break;
            case UnitState.Dead:

                break;

        }
        
        animator.SetInteger("State", (int)currentState);
    }
}
