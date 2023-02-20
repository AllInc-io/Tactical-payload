using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Gun : MonoBehaviour
{


    public GunData data;

    public bool stop = false;

    public float damageMultiplier = 1;
    [FoldoutGroup("Refs"), SerializeField] ParticleSystem muzzleFX;
    [FoldoutGroup("Refs"), SerializeField] public Transform bulletSource;
    [FoldoutGroup("Refs")] public Light lamp;
    [FoldoutGroup("Refs"), SerializeField] AudioSource audioSource;

    public float firerateMultiplier = 1;

    float timeSinceLastShot;

    public float bulletRange
    {
        get
        {
            return data.bulletSpeed * data.bulletLifetime;
        }
    }

    public bool isGrenade
    {
        get
        {
            return data.bulletPrefab.isGrenade;
        }
    }

    public void StartShooting()
    {
        if (!stop) return; 

        stop = false;
    }

    public void StopShooting()
    {
        stop = true;
        if (R.get.hasSFX && data.shootingClips != null && data.shootingClips.Length > 0) audioSource.PlayOneShot(data.shootingClips[Random.Range(0, data.shootingClips.Length)]);
    }

    //will shoot a bullet forward if timesincelastshot > rate
    //and returns wether it did shoot or not
    public bool TryShoot(bool tooClose = false)
    {

        if (data.bulletPrefab != null && data.bulletsPerSecond > 0 && timeSinceLastShot > 1f / (data.bulletsPerSecond * damageMultiplier))
        {
            ShootForward(tooClose);
            return true;
        }
        else return false;
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

    }


    public void ShootForward(bool fromOrigin)
    {
        timeSinceLastShot = 0;
        if(muzzleFX != null) muzzleFX.Play();
        for(int i = 0; i< data.bulletsPerShot; i++)
        {

            Bullet bullet = Instantiate(data.bulletPrefab);

            
            bullet.transform.position = fromOrigin ? transform.position : bulletSource.transform.position;
            bullet.transform.forward = Quaternion.Euler(0,Mathf.Lerp(-data.spread /2f, data.spread /2f, (float)i/ data.bulletsPerShot),0) * bulletSource.forward;
            bullet.speed = data.bulletSpeed;
            bullet.damage = Mathf.RoundToInt(data.damagePerBullet * damageMultiplier);
            bullet.lifetime = data.bulletLifetime;
            bullet.explosionRay = data.explosionRay;
            bullet.ennemiesLayerMask = data.ennemiesLayerMask;
            bullet.baseProjectionForce = data.baseProjectionForce;

            bullet.shooter = GetComponentInParent<Hero>();

            bullet.Init();
        }

        damageMultiplier = 1; //resets it
   
    }

}


[System.Serializable]
public struct GunData
{
    [SerializeField] public Bullet bulletPrefab;
    [SerializeField] public float bulletsPerSecond;
    [SerializeField] public float damagePerBullet;
    [SerializeField] public float bulletSpeed;
    [SerializeField] public float baseProjectionForce;

    [SerializeField] public float bulletsPerShot;
    [SerializeField] public float bulletLifetime;


    [SerializeField] public AudioClip[] shootingClips;

    [SerializeField] public float explosionRay;
    [HideInInspector] public LayerMask ennemiesLayerMask;

    [SerializeField, Range(1, 180)] public float spread;
}