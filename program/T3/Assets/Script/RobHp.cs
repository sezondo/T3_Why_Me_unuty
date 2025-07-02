using UnityEngine;

public class RobHp : MonoBehaviour
{
    private int currentHp;
    private RobBase robBase;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        robBase = GetComponent<RobBase>();

        animator = GetComponent<Animator>();
        
        currentHp = robBase.data.maxHp;
    }

    

    // Update is called once per frame
    void Update()
    {
        if (currentHp <= 0)
        {
            if (robBase.currentState == UnitState.Dead) return;

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
