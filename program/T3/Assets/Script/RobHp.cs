using UnityEngine;

public class RobHp : MonoBehaviour
{
    private int currentHp;
    private RobBase robBase;
    private Animator animator;
    bool die; //임시 조치
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        robBase = GetComponent<RobBase>();

        animator = GetComponent<Animator>();

        currentHp = robBase.data.maxHp;

        die = true;
    }



    // Update is called once per frame
    void Update()
    {

        switch (robBase.currentState)
        {
            case UnitState.Idle:

                break;

            case UnitState.Attacking:

                break;

            case UnitState.Moving:

                break;

            case UnitState.Dead:

                break;
            case UnitState.Turn:

                break;
            case UnitState.Hurt:

                break;

        }

        if (currentHp <= 0 && robBase.currentState != UnitState.Dead)
        {
            animator.SetTrigger("Death");
            currentHp = 0;
            robBase.ChangeState(UnitState.Dead);
        }
    }

    public void TakeDamage(int damage)
    {
        if (robBase.currentState == UnitState.Dead) return;


        currentHp -= damage;
    }
    
    
}
