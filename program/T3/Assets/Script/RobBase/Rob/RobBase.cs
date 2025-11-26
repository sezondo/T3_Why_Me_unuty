using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.AI;

public class RobBase : NetworkBehaviour
{
    public RobData data;
    public UnitState currentState{ get; private set; }
    protected Animator animator;
    protected Collider unitCollider;
    private Rigidbody rb;
    private NavMeshAgent navMeshAgent; //애니메이션용

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        currentState = UnitState.Idle;
        unitCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        rb.isKinematic = true;


        if (data.faction == FactionType.Ally)
        {
            gameObject.layer = LayerMask.NameToLayer("Ally");
        }
        else if(data.faction == FactionType.Enemy)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
        

    }

    // Update is called once per frame
    public virtual void Update()
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
                unitCollider.enabled = false; //나중에 고려해볼것 유닛 사망후 질질 끌려다니는거
                return;
            case UnitState.Turn:

                break;
        }

        animator.SetInteger("State", (int)currentState); // Attack 애니메이션은 트리거로 따로 관리
        /*
        if (navMeshAgent.velocity.sqrMagnitude < 0.01 && currentState != UnitState.Attacking)
        {
            animator.SetInteger("State", 0);
        }
        */
    }

    public void ChangeState(UnitState unitState)
    {
        if ( this.currentState == unitState) return;

        this.currentState = unitState;
    }
}
