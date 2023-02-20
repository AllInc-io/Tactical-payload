using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    [FoldoutGroup("Refs")] public Animator animator;
    [FoldoutGroup("Refs")] public Rigidbody rig;
    [FoldoutGroup("Refs")] public Collider col;
    [FoldoutGroup("Refs")] public Renderer meshRenderer;
    [FoldoutGroup("Refs"), SerializeField] ParticleSystem deathFX;
    [FoldoutGroup("Refs"), SerializeField] public Image lifeBar;
    [FoldoutGroup("Refs"), SerializeField] protected NavMeshAgent agent;


    //[FoldoutGroup("Refs")] public Shapes.Polyline line;

    [FoldoutGroup("Refs")] public Gun gun;
    [FoldoutGroup("Refs")] public ParticleSystem hitFx;

    [FoldoutGroup("Audio"), SerializeField] protected AudioSource audioSource;
    [FoldoutGroup("Audio"), SerializeField] protected AudioClip hurtClip;
    [FoldoutGroup("Audio"), SerializeField] protected AudioClip deadClip;

    [SerializeField] public float speed = 5f;


    [SerializeField] public GunData startGunData;

    Color initColor;

    [SerializeField] public LayerMask obstacles;

    public bool dead = false;
    [SerializeField] protected int maxPVs;

    protected float PVs;

    protected float speedMultiplier = 1;

    public int scoreUnlock = 0;

    public void Awake()
    {
        if (lifeBar != null)
        {
            lifeBar.transform.parent.localScale = Vector3.zero;
            lifeBar.transform.parent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }


    public virtual void Init()
    {

        PVs = maxPVs;
        Collider[] arrCol = animator.transform.GetComponentsInChildren<Collider>();
        for (int i=0; i < arrCol.Length; i++)
        {
            arrCol[i].enabled = false;
        } 

        Rigidbody[] arrRig = animator.transform.GetComponentsInChildren<Rigidbody>();
        for (int i=0; i < arrRig.Length; i++)
        {
            arrRig[i].isKinematic = true;
        }

        if(gun != null) gun.data = startGunData;
        //if(meshinitColor = meshRenderer.material.color;


    }

    public virtual void SlowDown(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    public bool IsAtMaxLife()
    {
        return PVs == maxPVs;
    }

    public void ActivateRagdoll(Vector3 force)
    {
        animator.Play("Idle");


        col.enabled = false;
        if(rig != null) rig.isKinematic = true;
        animator.enabled = false;
        
        Collider[] arrCol = animator.transform.GetComponentsInChildren<Collider>();
        for (int i=0; i < arrCol.Length; i++)
        {
            arrCol[i].enabled = true;
            arrCol[i].isTrigger = false;
        } 

        Rigidbody[] arrRig = animator.transform.GetComponentsInChildren<Rigidbody>();
        for (int i=0; i < arrRig.Length; i++)
        {
            arrRig[i].velocity = Vector3.zero;
            arrRig[i].isKinematic = false;
            arrRig[i].interpolation = RigidbodyInterpolation.Interpolate;
            arrRig[i].AddForce(force * Random.Range(0.9f, 1.1f) * 0.5f, ForceMode.Impulse);
        } 

    }

    protected void DeactivateRagdoll()
    {
        animator.Play("Idle");


        col.enabled = true;
        if (rig != null) rig.isKinematic = true;
        animator.enabled = true;

        Collider[] arrCol = animator.transform.GetComponentsInChildren<Collider>();
        for (int i = 0; i < arrCol.Length; i++)
        {
            arrCol[i].enabled = false;
            arrCol[i].isTrigger = false;
        }

        Rigidbody[] arrRig = animator.transform.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < arrRig.Length; i++)
        {
            arrRig[i].velocity = Vector3.zero;
            arrRig[i].isKinematic = true;
            arrRig[i].interpolation = RigidbodyInterpolation.Interpolate;
            
        }

    }
    public virtual void StopMoving()
    {

        SetIdle();

    }

    protected virtual void MoveTowards(Vector3 goal, float tempSpeedModifier = 1)
    {
        animator.SetTrigger("Run");


        if (Vector3.Distance(transform.position, goal) <= Time.deltaTime * (speed * speedMultiplier * tempSpeedModifier))
        {
            transform.position = goal;
        }
        else
        {

            transform.position += (goal - transform.position).normalized * Time.deltaTime * (speed * speedMultiplier * tempSpeedModifier);
        }
        speedMultiplier = 1; //it was consumed and will be changed again next frame if needed


    }

    public void SetIdle()
    {
        animator.ResetTrigger("Run");
        animator.Play("Idle");
    }

    protected virtual void Update()
    {
        //if (dead) return;
        lifeBar.transform.parent.forward = -R.get.mainCamera.transform.forward;
    }

    
    private void LateUpdate()
    {
        //resets the multiplier, will be re-set to value by heal/boost if necessary

    }


    public virtual void TakeDamage(float amount, Vector3 ragdollForce)
    {

        PVs -= amount;
        UpdateLifeBar();


        if (PVs <= 0) Die(ragdollForce * Mathf.Min(amount, R.get.levelDesign.maxZombiesProjectionForceWhenKilled));

        if (R.get.hasSFX && hurtClip != null) audioSource.PlayOneShot(hurtClip);

        /*if(meshRenderer != null)
        {
            DOTween.Kill(meshRenderer.material);
            meshRenderer.material.DOColor(Color.white, 0.3f).OnComplete(() => meshRenderer.material.DOColor(initColor, 0.3f));
        }*/
    }


    protected void UpdateLifeBar()
    {

        if ( !lifeBar.transform.parent.gameObject.activeInHierarchy) lifeBar.transform.parent.gameObject.SetActive(true);
        DOTween.Kill(lifeBar, "FillLifeCircle");
        lifeBar.DOFillAmount(PVs /(float)maxPVs, 0.3f).SetId("FillLifeCircle");
    }


    protected virtual void Die(Vector3 ragdollForce)
    {

        if (R.get.hasSFX && deadClip != null) audioSource.PlayOneShot(deadClip);

        dead = true;
        if(gun != null) gun.StopShooting();
        if (deathFX != null) Instantiate(deathFX, transform.position, default);
 
        ActivateRagdoll(ragdollForce);

        if (lifeBar != null)
        {
            DOTween.Kill("LifeBarScaleTween");
            lifeBar.transform.parent.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetId("LifeBarScaleTween");
        }

    }
}
