using UnityEngine;
using System.Collections;
using Fusion;
public class RobHp : NetworkBehaviour
{
    public int currentHp;
    protected RobBase robBase;
    protected Animator animator;
    //[SerializeField] private AudioClip dieAudioClip;

    bool die; //임시 조치
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        robBase = GetComponent<RobBase>();

        animator = GetComponent<Animator>();

        currentHp = robBase.data.maxHp;

        die = true;
    }



    // Update is called once per frame
    public virtual void Update()
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

        }

        if (currentHp <= 0 && robBase.currentState != UnitState.Dead)
        {
            DeadDisposal();
        }
    }
    public virtual void DeadDisposal() // 죽음 효과 처리 시작 + 애니메이션 트리거
    {
        animator.SetTrigger("Death");
        currentHp = 0;
        robBase.ChangeState(UnitState.Dead); // 클라에서 중복이 나긴 하지만 RobBase에서 어차피 막자나 이거 괜찮을듯
        StartCoroutine(TakeDieWait());
    }

    public virtual void TakeDamage(int damage)
    {
        if (robBase.currentState == UnitState.Dead) return;


        currentHp -= damage;
    }

    public virtual IEnumerator TakeDie()
    {

        float time = 0f;
        float duration = 5f;
        float speed = 3f;

        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * speed;

        while (time < duration)
        {
            gameObject.transform.position = Vector3.Lerp(
            start,
            end,
            time / duration);

            time += Time.deltaTime;
            yield return null;
        }

        Destroy();
        transform.position = end;
    }

    public IEnumerator TakeDieWait()
    {
        SoundManager.instance.PlaySFX(robBase.data.dieAudioClip, transform);

        yield return new WaitForSeconds(5f);

        StartCoroutine(TakeDie());
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}
