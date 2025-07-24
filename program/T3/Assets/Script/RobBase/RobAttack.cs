using UnityEngine;
using System.Collections;

public class RobAttack : MonoBehaviour
{
    public RobBase robBase;
    public Animator animator;
    private RobStooter[] shooter;
    public bool CoroutineCheck;
    [SerializeField] private AudioClip attackAudioClip;
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

    public virtual IEnumerator Attacking()
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

    public void Fire() {
        SoundManager.instance.PlaySFX(attackAudioClip, this.transform);
        foreach (var fp in shooter)
        {
            fp.Stoot();
        }
    }
    


}
