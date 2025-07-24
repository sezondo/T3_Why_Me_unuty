using UnityEngine;

public class Bullets : MonoBehaviour
{
    private float speed;
    private float lifeTime;
    private int damage;
    private FactionType factionType;
    public BulletData bulletData;
    [SerializeField] private GameObject hitPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        speed = bulletData.bulletSpeed;
        lifeTime = bulletData.bulletLifeTime;
        damage = bulletData.bulletDamage;

        factionType = bulletData.faction;

        if (bulletData.faction == FactionType.Ally)
        {
            gameObject.layer = LayerMask.NameToLayer("Ally");
        }
        else if(bulletData.faction == FactionType.Enemy)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }

        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == gameObject.layer)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            DestroyBullet();
            return;
        }

        Debug.Log("총알 충돌 대상: " + other.name + ", 태그: " + other.tag);

        if (other.gameObject.layer != gameObject.layer)
        {
            RobHp robHp = other.GetComponent<RobHp>();
            if (robHp != null)
            {
                robHp.TakeDamage(damage);
                DestroyBullet();
            }
        }

    }

    public virtual void DestroyBullet()
    {
        Destroy(gameObject);
        //여따가 사운드 매니저랑 이펙트 매니저 넣을껏
        EffectManager.instance.PlayEffecting(hitPrefab, this.transform);
    }
}
