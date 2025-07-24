using DG.Tweening;
using UnityEngine;

public class Rob04Bullets : Bullets
{
    [SerializeField] private AudioClip destAudioClip;
    private Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();

        Vector3 midPoint = (transform.position + target.position) / 2f;
        midPoint += Vector3.up * base.bulletData.arcHeight;

        Vector3[] path = new Vector3[] {
            transform.position,
            midPoint,
            target.position
        };

        float distance = Vector3.Distance(transform.position, target.position);
        float duration = distance / base.bulletData.bulletSpeed;

        transform.DOPath(path, duration, PathType.CatmullRom)
        .SetEase(Ease.Linear)
        .SetLookAt(0.01f)
        .OnComplete(() =>
        {
            DestroyBullet();
        });
    }

    public override void DestroyBullet()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, base.bulletData.explosionRadius);

        foreach (Collider hit in hits)
        {
            RobHp hp = hit.GetComponent<RobHp>();
            if (hp != null)
            {
                hp.TakeDamage(base.bulletData.bulletDamage);
            }
        }

        SoundManager.instance.PlaySFX(destAudioClip, this.transform);

        base.DestroyBullet();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, base.bulletData.explosionRadius);

    }

    public void TargetPoint(Transform transform)
    {
        target = transform;
    }

    // Update is called once per frame
    public override void Update()
    {
        //트랜스폼 이동 삭제
    }
    public override void OnTriggerEnter(Collider other)
    {
        //이놈도 삭제
    }
}
