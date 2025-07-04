using UnityEngine;

public class RobBase : MonoBehaviour
{
    public RobData data;
    public UnitState currentState;
    private Animator animator;
    private Collider unitCollider;
    public Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        currentState = UnitState.Idle;
        unitCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;


        if (data.faction == FactionType.Ally)
        {
            gameObject.layer = 10;
        }
        else if(data.faction == FactionType.Enemy)
        {
            gameObject.layer = 20;
        }
        

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
                //unitCollider.enabled = false; 나중에 고려해볼것 유닛 사망후 질질 끌려다니는거
                return;
            case UnitState.Turn:

                break;


        }

        animator.SetInteger("State", (int)currentState); // Attack 애니메이션은 트리거로 따로 관리
    }

    public void ChangeState(UnitState unitState)
    {
        if ( this.currentState == unitState) return;

        this.currentState = unitState;
    }
}
