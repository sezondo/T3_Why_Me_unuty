using System.Collections;
using UnityEngine;

public class Rob04Attack : RobAttack
{
    public override IEnumerator Attacking()
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
            
            animator.SetTrigger("Attack");

            yield return new WaitForSeconds(0.8f);

            Fire();

            

            yield return new WaitForSeconds(robBase.data.attackSpeed - 0.8f);
        }
        CoroutineCheck = false;
    }
}
