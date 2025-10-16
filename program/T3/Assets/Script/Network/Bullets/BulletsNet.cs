using UnityEngine;
using System.Collections;
using Fusion;

public class BulletsNet : Bullets
{
    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            StopAllCoroutines();
            this.enabled = false;
            return;
        }
        base.Start();

        StartCoroutine(DelayedStart());
    }

    public override void Start(){ }

    private IEnumerator DelayedStart()
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
        if (Object.HasStateAuthority)
        {
            transform.Translate(Vector3.forward * speed * Runner.DeltaTime);
        }
        
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
        RPC_DestroyBullet();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DestroyBullet()
    {
        EffectManager.instance.PlayEffecting(hitPrefab, this.transform);

        if (Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }
    
}
