using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{

    [SerializeField] ParticleSystem destructionFXPrefab;
    [SerializeField] int PVs;
    [SerializeField] float boostDropProbability;
    [SerializeField] Bonus[] possibleBoosts;
    
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
    }

    private void OnDestroyed()
    {
        if(Random.value <= boostDropProbability)
        {
            Vector3 pos = transform.position;
            pos.y = 0.5f;
            Instantiate(possibleBoosts[Random.Range(0, possibleBoosts.Length)], pos, default);
        }

        Instantiate(destructionFXPrefab, transform.position, default);
        Destroy(this.gameObject);
    }
}
