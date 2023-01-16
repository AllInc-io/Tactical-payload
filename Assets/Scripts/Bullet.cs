using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Bullet : MonoBehaviour
{
    public float speed;
    public int damage;
    public float lifetime = 5f;
    public LayerMask canCollideWith;
    public bool isExplosive;
    public bool goesThroughEnemies;
    public bool isGrenade;
    public float setsOnFireCounter = 0;
    public float baseProjectionForce;
    [ShowIf("isGrenade")] public AnimationCurve grenadeCurve;

    [SerializeField] public float explosionRay; //set by gunner's explosion ray
    [HideInInspector] public LayerMask ennemiesLayerMask; //also set by gunner

    [SerializeField] ParticleSystem onKillFx;


    float counter;
    // Update is called once per frame

    Vector3 initPos;
    Vector3 finalPos;

    public void Init()
    {
        if (isGrenade)
        {
            initPos = transform.position;

            finalPos = transform.position + transform.forward * speed * lifetime;

  
            finalPos.y = 0;

        }
    }


    void Update()
    {

        counter += Time.deltaTime;

        if (counter <= lifetime)
        {
            if (isGrenade)
            {
                Vector3 newPos = grenadeCurve.Evaluate(counter/lifetime) * Vector3.up + Vector3.Lerp(initPos, finalPos, counter / lifetime);
                transform.Rotate(new Vector3(10, 10, 10));
                transform.position = newPos;
            }
            else if (!CheckForCollision(out RaycastHit hit))
            {

                transform.position += transform.forward * speed * Time.deltaTime;
            }
        }
        else Kill();

    }

    public void Kill()
    {
        if (onKillFx != null) Instantiate(onKillFx, transform.position, default);

        if(isExplosive)
        {
            Collider[] results = Physics.OverlapSphere(transform.position, explosionRay, ennemiesLayerMask);
            foreach(Collider result in results)
            {
                result.GetComponent<Character>().TakeDamage(damage, transform.position - result.transform.position);
            }
        }
        //should be pooled later
        Destroy(this.gameObject);
    }

    public bool CheckForCollision(out RaycastHit collidedWith)
    {
        return Physics.Raycast(transform.position, transform.forward, out collidedWith, speed * Time.deltaTime, canCollideWith);
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Obstacle")) Kill();
    }
}
