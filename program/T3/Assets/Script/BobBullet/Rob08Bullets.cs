using System.Collections.Generic;
using UnityEngine;
using System.Collections;


public class Rob08Bullets : Bullets
{
    public ParticleSystem flameParticleFire;
    public ParticleSystem flameParticleSmoke;
    public ParticleSystem flameParticleBits;
    private List<GameObject> damagedTargets;
    [HideInInspector] public float attackTime;
    public AudioClip audioClipFire;
    public AudioSource audioSource;

    public void OnEnable()
    {
        if (flameParticleFire != null)
        {
            flameParticleFire.Play();
        }
        if (flameParticleSmoke != null)
        {
            flameParticleSmoke.Play();
        }
        if (flameParticleBits != null)
        {
            flameParticleBits.Play();
        }
        audioSource.Play();
    }
    public override void Start()
    {
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
        damagedTargets = new List<GameObject>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != this.gameObject.layer)
        {
            if (!damagedTargets.Contains(other.gameObject))
            {
                RobHp robHp;
                robHp = other.GetComponent<RobHp>();
                if (robHp != null)
                {
                    robHp.TakeDamage(damage);
                    damagedTargets.Add(other.gameObject);
                    StartCoroutine(RemoveFromListAfterDelay(other.gameObject, attackTime));
                }
            }
        }
    }

    private IEnumerator RemoveFromListAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        damagedTargets.Remove(target);
    }

    public IEnumerator stopFire()
    {
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);
    }

    public override void OnTriggerEnter(Collider other)
    {

    }
    public override void DestroyBullet()
    {

    }
    public override void Update()
    {
        
    }
}
