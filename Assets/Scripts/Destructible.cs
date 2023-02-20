using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Destructible : MonoBehaviour
{

    [SerializeField] ParticleSystem destructionFXPrefab;
    [SerializeField] int PVs;
    [SerializeField] float boostDropProbability;
    [SerializeField] Bonus[] possibleBoosts;


    [SerializeField] public bool explodes;
    [SerializeField, ShowIf("explodes")] float explosionRadius;
    [SerializeField, ShowIf("explodes")] int explosionDamage;
    [SerializeField, ShowIf("explodes")] LayerMask ennemiesLayermask;
    [SerializeField, ShowIf("explodes")] LayerMask destructiblesLayermask;




    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            PVs -= other.GetComponentInParent<Bullet>().damage;
            Destroy(other.gameObject);
            if (PVs <= 0)
            {
                OnDestroyed();
            }

        }
        else if(other.CompareTag("Destructible"))
        {
            //to avoid two crates bugging into each other
            Destroy(this.gameObject);
        }
    }

    private void OnDestroyed()
    {
        if(Random.value <= boostDropProbability)
        {
            Vector3 pos = transform.position;
            pos.y = 0.5f;
            Instantiate(possibleBoosts[Random.Range(0, possibleBoosts.Length)], pos, default);
        }

        if(explodes)
        {
            Collider[] results = Physics.OverlapSphere(transform.position, explosionRadius, ennemiesLayermask, QueryTriggerInteraction.Collide);

            //kill ennemies
            foreach(Collider hit in results)
            {
                hit.GetComponent<Enemy>().TakeDamage(explosionDamage, (hit.transform.position - transform.position).normalized * Vector3.Distance(hit.transform.position,transform.position));
            }

            //destroy destructibles in radius
            results = Physics.OverlapSphere(transform.position, explosionRadius, destructiblesLayermask, QueryTriggerInteraction.Collide);
            foreach (Collider hit in results)
            {
                hit.GetComponent<Destructible>().DestroyByExplosion(0.3f);
            }
        }
        destructionFXPrefab.transform.SetParent(null);
        destructionFXPrefab.gameObject.SetActive(true);
        Destroy(this.gameObject);
    }


    public void DestroyByExplosion(float delay)
    {
        StartCoroutine(DestroyByExplosionCoroutine(delay));
    }

    IEnumerator DestroyByExplosionCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnDestroyed();
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (explodes) Gizmos.DrawLine(transform.position, transform.position + Vector3.right * explosionRadius);
    }
}
