using UnityEngine;
using System.Collections;
using Fusion;

public class BulletsNet : Bullets
{
    [Networked] protected TickTimer DespawnTimer { get; set; }
    protected bool pendingDespawn;
    protected Vector3 pendingHitPos;
    
    
    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            StopAllCoroutines();
            //this.enabled = false;
            return;
        }
        base.Start();

        StartCoroutine(DelayedStart());
    }

    public override void Start(){ }

    protected IEnumerator DelayedStart()
    {

        speed = bulletData.bulletSpeed;
        lifeTime = bulletData.bulletLifeTime;
        damage = bulletData.bulletDamage;

        factionType = bulletData.faction;

        if (bulletData.faction == FactionType.Ally)
        {
            gameObject.layer = LayerMask.NameToLayer("Ally");
        }
        else if (bulletData.faction == FactionType.Enemy)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }

        yield return new WaitForSeconds(lifeTime);

        Runner.Despawn(Object);
    }

    public override void FixedUpdateNetwork()
    {

        if (!Object.HasStateAuthority)
            return;

        if (pendingDespawn)
        {
            if (DespawnTimer.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
                return; // 더 이상 이동시키지 않음   
        }

        transform.Translate(Vector3.forward * speed * Runner.DeltaTime);
        
    }

    public override void Update()
    {
        return;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }

        if (other.gameObject.layer == gameObject.layer)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            DestroyBullet();
            return;
        }

        //Debug.Log("총알 충돌 대상: " + other.name + ", 태그: " + other.tag);

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
    
    public override void DestroyBullet()
    {
        if (!Object.HasStateAuthority) return;
        pendingHitPos = transform.position;

        RPC_DestroyBullet(pendingHitPos);
        //NetworkBulletManager.instance.RPC_PlayerEffect(hitPrefab, pendingHitPos);

        pendingDespawn = true;
        DespawnTimer = TickTimer.CreateFromSeconds(Runner, 0.05f); // 한 틱 지연
    
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_DestroyBullet(Vector3 hitPos)
    {
        if (EffectManager.instance != null && hitPrefab != null)
        {
            EffectManager.instance.PlayEffect(hitPrefab, hitPos);
        }
    }
}
