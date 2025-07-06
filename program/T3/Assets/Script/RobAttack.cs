using UnityEngine;
using System.Collections;

public class RobAttack : MonoBehaviour
{
    private RobBase robBase;
    private Animator animator;
    private RobStooter[] shooter;
    private bool CoroutineCheck;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        robBase = GetComponent<RobBase>();
        shooter = GetComponentsInChildren<RobStooter>();
    }

    // Update is called once per frame
    void Update()
    {

        if (robBase.currentState == UnitState.Dead)
        {
            return;
        }

        switch (robBase.currentState)
        {
            case UnitState.Idle:

                break;

            case UnitState.Attacking:

                if (!CoroutineCheck)
                {
                    CoroutineCheck = true;
                    StartCoroutine(Attacking());
                }
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
    }

    private IEnumerator Attacking()
    {
        while (true)
        {
            if (robBase.currentState == UnitState.Dead)
            {
                break;
            }

            if (robBase.currentState != UnitState.Attacking)
            {
                break;
            }

            Fire();

            animator.SetTrigger("Attack");

            yield return new WaitForSeconds(robBase.data.attackSpeed);
        }
        CoroutineCheck = false;
    }

    private void Fire() {
        foreach (var fp in shooter)
        {
            fp.Stoot();
        }
    }
    


}
