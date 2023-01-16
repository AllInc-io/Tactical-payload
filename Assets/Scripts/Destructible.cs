using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{

    [SerializeField] ParticleSystem destructionFXPrefab;
    [SerializeField] int PVs;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            PVs -= other.GetComponentInParent<Bullet>().damage;
            Destroy(other.gameObject);
            if (PVs <= 0)
            {
                Instantiate(destructionFXPrefab, transform.position, default);
                Destroy(this.gameObject);
            }

        }
    }
}
