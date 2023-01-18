using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.AI;

public class Enemy : Character
{



    [OnValueChanged("SetUpVisionCone")] [SerializeField, Range(1, 180)] float visionAngle = 90f;
    [OnValueChanged("SetUpVisionCone")] [SerializeField] float visionDistance = 8;
    [SerializeField] Polygon visionCone;
    [SerializeField] LayerMask heroes;

    [SerializeField] EnemyType type;
    [SerializeField, ShowIf("type", EnemyType.Tourelle)] float lookAroundEveryXSeconds = 5f;

    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackEveryXSeconds = 2f;

    [SerializeField] bool isRunner;
    [SerializeField] public string enemyName;
    [SerializeField] int damagePerHit = 1;

    [SerializeField] Transform head;

    [SerializeField] Zombie[] possibleZombies;

    [SerializeField] ParticleSystem onFireFx;

    public Zombie zombie;

    [SerializeField] float runningSpeed = 5f;

    protected float speedRandomMultiplier = 1;

    Vector3 previousPos;


    float noMovingCounter;
    
    int favoriteWay = 1;

    bool sawSomething = false;

    [SerializeField] CollectibleCoin coin;

    public bool isCrawling;

    //new public Transform transform;

    float attackCounter;
    float lookAroundCounter;
    bool lookingAround = false;

    Coroutine walkingAroundCoroutine;

    bool isInit = false;

    public Light entranceLight;

    public float onFireCounter;

    string movementString;

    public void Preinit() //spawns the zombie
    {

        zombie.transform.localPosition = Vector3.zero;
        zombie.transform.forward = transform.forward;
        //zombie.transform.localScale = Vector3.one * 1.5f;
        animator = zombie.GetComponentInChildren<Animator>();
        animator.enabled = true;


    }

    public override void Init()
    {


        dead = false;
        onFireCounter = 0;
        col.enabled = true;
        if (lifeCircle != null) lifeCircle.transform.localScale = Vector3.one;

        if (animator == null) Preinit();

        animator.enabled = true;

        foreach (Collider collider in GetComponentsInChildren<Collider>())
        {
            collider.isTrigger = true;
        }

        PVs = maxPVs;

        speedRandomMultiplier = Random.Range(0.95f, 1.05f);

        agent.enabled = true;

        base.Init();

        if (isRunner) movementString = "Run";
        else if (isCrawling) movementString = "Crawl";
        else movementString = "Walk";

        if (type == EnemyType.Tourelle)
        {
            animator.Play("Idle");
            //InitVisionCone();

        }
        else if (type == EnemyType.Walker)
        {
            animator.Play("Idle");
            //InitVisionCone();
        }
        else if (type == EnemyType.Crawler)
        {
            animator.SetTrigger(movementString);
            StartCoroutine(UpdateDestinationCoroutine());
        }

        favoriteWay = U.RandomSign();
        
        //StartCoroutine(LookAroundCoroutine());

        zombie.ResetVisuals();

        isInit = true;

        
    }

    protected override void Update()
    {

        base.Update();

        if (dead || !R.get.game.isOn || !isInit) return;

        if (Vector3.Distance(previousPos, transform.position) < 0.1f) //a set value of "min movement"
        {
            noMovingCounter += Time.deltaTime;
            if (noMovingCounter > 8f)
            {
                Die(Vector3.zero);
                return;
            }
        }
        else
        {
            noMovingCounter = 0;
            previousPos = transform.position;
        }

        if(onFireCounter > 0)
        {
            onFireFx.gameObject.SetActive(true);
            TakeDamage(R.get.levelDesign.damagePerSecondWhenOnFire * Time.deltaTime, Vector3.zero);
        }
        else
        {
            onFireFx.gameObject.SetActive(false);
        }

        attackCounter += Time.deltaTime;


    }


    IEnumerator UpdateDestinationCoroutine()
    {
        while(!dead)
        {
            if (type == EnemyType.Crawler)
            {
                agent.speed = speed * speedMultiplier * speedRandomMultiplier;
                LookForInterestPoint(out Vector3 goal);
                
                if (Vector3.Distance(transform.position, goal) >= attackRange)
                {
                    //Debug.LogWarning("1");
                    if (Vector3.Distance(agent.destination, goal) > 0.2f)
                    {
                        agent.SetDestination(goal);
                    }


                }
                else
                {


                        agent.SetDestination(transform.position);
                        Attack();
                    

                }


                //MoveTowards(goal);

            }

            //resets speedMultiplier because it was "used", will be re-set to another value if still in a modifier's range
            speedMultiplier = 1;
            yield return new WaitForSeconds(0.3f);
        }
        
    }

    void Attack()
    {

        if (attackCounter < attackEveryXSeconds) return;

        attackCounter = 0;
        //hurts every character in range
        animator.SetTrigger(isCrawling ? "AttackGround" : "Attack");
        Collider[] results = Physics.OverlapSphere(transform.position, attackRange, heroes);
        if(results.Length > 0)
        {
            foreach(Collider result in results)
            {
                if(result.GetComponent<Hero>() != null) result.GetComponent<Hero>().TakeDamage(damagePerHit, Vector3.zero);
            }
        }
        if (Vector3.Distance(R.get.levelManager.level.payload.transform.position, transform.position) < attackRange) R.get.levelManager.level.payload.TakeDamage(1);

    }


    void InitVisionCone()
    {

        visionCone.transform.SetParent(null);
        visionCone.transform.localScale = Vector3.one;
        visionCone.transform.localRotation = Quaternion.Euler(90, 0, 0);
        visionCone.transform.position = Vector3.zero + Vector3.up * 0.4f;

        lookAroundCounter = Random.Range(0, lookAroundEveryXSeconds);
    }

    Vector3 FindClosestPlayer()
    {
        Character closestChara;
        int i = 0;
        do
        {
            closestChara = R.get.game.heroes[i];
            i++;
        } while (closestChara.dead && i < R.get.game.heroes.Length);

        Vector3 closestPlayer = closestChara.transform.position;
        foreach(Character chara in R.get.game.heroes)
        {
            if(!chara.dead && Vector3.Distance(closestPlayer, transform.position) > Vector3.Distance(chara.transform.position, transform.position))
            {
                closestPlayer = chara.transform.position;
            }
        }
        return closestPlayer;
    }



    bool LookForInterestPoint(out Vector3 target)
    {
        target = R.get.levelManager.level.payload.transform.position + R.get.levelManager.level.payload.transform.forward * attackRange / 2f;

        Collider[] results = Physics.OverlapSphere(transform.position, visionDistance, heroes, QueryTriggerInteraction.Collide);
            if (results != null && results.Length > 0 &&
                !results[0].GetComponent<Hero>().dead 
                && !Physics.Raycast(transform.position + Vector3.up * 0.2f, results[0].transform.position - transform.position, visionDistance * transform.lossyScale.z, obstacles, QueryTriggerInteraction.Collide))
            {
                target = results[0].transform.position;
            }
        
        return target != null;

    }

    public IEnumerator WalkAroundCoroutine()
    {
        Vector3 startForward = transform.forward;
        Vector3 origin = transform.position;

        float newX = Mathf.Clamp(transform.position.x > 0 ? transform.position.x - R.get.levelManager.level.width * 0.6f : transform.position.x + R.get.levelManager.level.width * .6f, -R.get.levelManager.level.width /2f + 0.5f, R.get.levelManager.level.width/2f - 0.5f);
        Vector3 goal = new Vector3(newX, transform.position.y, transform.position.z);

        float t = 0;
        lookingAround = true;

        agent.SetDestination(goal);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, goal) <= 0.05f);

        agent.SetDestination(origin);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, origin) <= 0.05f);

        /*
        while (t <= 1)
        {
            transform.forward = Vector3.Lerp(startForward, (goal - origin).normalized, t);
            t += Time.deltaTime * 2f;
            yield return null;
        }
        t = 0;
        while (Vector3.Distance(transform.position, goal) >= 0.01f && t <= 5f)
        {
            MoveTowards(goal);
            transform.forward = goal - transform.position;
            t += Time.deltaTime;
            yield return null;
        }
        t = 0;
        while(t <= 1)
        {
            transform.forward = Vector3.Lerp(startForward, (origin - goal).normalized, t);
            t += Time.deltaTime * 2f;
            yield return null;
        }
        yield return new WaitForSeconds(lookAroundEveryXSeconds);
        t = 0;
        while (Vector3.Distance(transform.position, origin) >= 0.01f && t <= 5f)
        {
            MoveTowards(origin);
            transform.forward = origin - goal;
            t += Time.deltaTime;
            yield return null;
        }
        t = 0;
        while (t <= 1)
        {
            transform.forward = Vector3.Lerp((origin - goal).normalized, startForward, t);
            t += Time.deltaTime * 2f;
            yield return null;
        }*/
        lookingAround = false;
        SetIdle();
    }

    List<Vector2> points;
    void SetUpVisionCone()
    {

        points = new List<Vector2>();
        //Vector2[] points = new Vector2[Mathf.RoundToInt(visionAngle + 1)];
        //points[0] = new Vector2(transform.position.x, transform.position.z);
        points.Add(new Vector2(transform.position.x, transform.position.z));
        Vector3 startDir = Quaternion.Euler(0, -visionAngle / 2f, 0) * head.forward;
        for (int i = 0; i < visionAngle; i++)
        {
            Vector3 dir = Quaternion.Euler(0, i, 0) * startDir;
            Vector2 newPoint;
            if (Physics.Raycast(transform.position + Vector3.up * 0.6f, dir, out RaycastHit hit, visionDistance * transform.lossyScale.z, obstacles, QueryTriggerInteraction.Collide))
            {

                newPoint = points[0] + new Vector2(dir.x, dir.z).normalized * Vector3.Distance(transform.position, hit.point);
                //points[i + 1] = points[0] + new Vector2(dir.x, dir.z).normalized * Vector3.Distance(transform.position, hit.point);
            }
            else
            {
                newPoint = points[0] + transform.lossyScale.z * visionDistance * new Vector2(dir.x, dir.z).normalized;
            }
            if (Vector2.Distance(newPoint, points[points.Count - 1]) > 0.3f) points.Add(newPoint);


        }

        //visionCone.SetPoints(points);
        //float angle = visionAngle / 2f;
        //visionCone.innerSpotAngle = visionDistance * Mathf.Tan(((angle) * Mathf.Deg2Rad) / 2f) * 2f;

        //visionCone.transform.localPosition = new Vector3(0, visionCone.transform.position.y, visionDistance);
    }

    void LookAround(float duration)
    {
        lookingAround = true;
        DOTween.Kill(this, "LookAround");
        Quaternion startRotation = head.rotation;
        Sequence sequence = DOTween.Sequence(this).SetId("LookAround").OnComplete(() => lookingAround = false);
        sequence.Append(head.DORotate(startRotation * Vector3.up * 90f, duration *0.4f));
        sequence.Append(head.DORotate(startRotation * Vector3.up * -90f, duration * 0.4f));
        sequence.Append(head.DORotate(startRotation.eulerAngles, duration * 0.2f));

    }

    protected override void MoveTowards(Vector3 goal, float tempSpeedModifier = 1f)
    {
        if(type != EnemyType.Crawler) animator.SetTrigger("Run");
        Debug.LogWarning("Im calling movetowards :(");
        agent.SetDestination(goal);
        /*
        //check if there is an obstacle in front
        if (Physics.Raycast(transform.position + (goal - transform.position).normalized * transform.localScale.y, goal - transform.position, out RaycastHit hit, Time.deltaTime * (speed * speedMultiplier) + 0.3f, obstacles))
        {
            Vector3 dir = hit.normal;


            dir = new Vector3(favoriteWay * dir.z, 0, -favoriteWay * dir.x);
            //dir = Vector3.Angle(dir, goal - transform.position) < 10f || Vector3.Angle(goal - transform.position, dir1) <= Vector3.Angle(goal - transform.position, dir2) ? dir1: dir2;
            transform.position += (dir * Time.deltaTime * (speed * speedMultiplier));
            transform.forward = dir;
        }
        else
        {
            if (Vector3.Distance(transform.position, goal) <= Time.deltaTime * (speed * speedMultiplier))
            {
                transform.position = goal;
            }
            else
            {

                transform.position += (goal - transform.position).normalized * Time.deltaTime * (speed * speedMultiplier);
            }
            transform.forward = goal - transform.position;
        }
        */
        speedMultiplier = 1; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (R.get.game.isOn && !dead || !isInit)
        {
            if (other.CompareTag("Bullet"))
            {
                //Debug.Log("touch�");
                Bullet bullet = other.GetComponent<Bullet>();
                TakeDamage(bullet.damage, bullet.transform.forward * bullet.baseProjectionForce);
                Instantiate(hitFx, bullet.transform.position, default).gameObject.SetActive(true);
                if (bullet.setsOnFireCounter > 0) onFireCounter = bullet.setsOnFireCounter;
                if(!bullet.goesThroughEnemies) bullet.Kill();
            }

        }


    }

    protected override void Die(Vector3 ragdollForce)
    {
        base.Die(ragdollForce);


        StopAllCoroutines();
        col.enabled = false;
        agent.enabled = false;
        //zombie.Explode();
        onFireFx.gameObject.SetActive(false);


        if (Random.value <= 0.1f)
        {
            //Instantiate(coin, transform.position + Vector3.up * 0.5f, default);
        }

        StartCoroutine(FullyDisableCoroutine(2f));


        //meshRenderer.material.DOColor(Color.white, 0.5f).OnComplete(() => Destroy(this.gameObject, 0.5f));
        if (visionCone != null) visionCone.enabled = false;

        R.get.levelManager.level.enemiesAlive--;
        R.get.levelManager.level.totalEnemiesKilled++;


    }


    private IEnumerator FullyDisableCoroutine(float delay)
    {
        float t = 0;
        while(t <= 1)
        {
            zombie.SetDissolveValue(t);
            
            t += Time.deltaTime / delay;
            yield return null;
            
        }

        R.get.levelManager.level.enemiesPool.Add(this);
        gameObject.SetActive(false);


    }

    private void OnDrawGizmos()
    {
        /*if (Vector3.Distance(agent.destination, transform.position) < 0.3f) return;

        foreach(Vector2 point in agent.path.corners)
        {
            Gizmos.DrawSphere(point, 0.2f);
        }*/

        Gizmos.color = Color.magenta;
        if (agent != null)
        {
            Gizmos.DrawLine(transform.position, agent.destination);
            Gizmos.DrawWireSphere(agent.destination + Vector3.up, 0.3f);
        }
    }
}

enum EnemyType
{
    Tourelle, 
    Walker, 
    Crawler
}