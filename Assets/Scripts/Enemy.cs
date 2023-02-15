using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.AI;
using TMPro;
public class Enemy : Character
{



    [SerializeField] RectTransform UIPositionIndicatorPrefab;

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
    [SerializeField] TextMeshPro expGainedText;

    public Zombie zombie;

    [SerializeField] float runningSpeed = 5f;

    protected float speedRandomMultiplier = 1;

    Vector3 previousPos;


    float noMovingCounter;
    
    int favoriteWay = 1;

    bool sawSomething = false;

    [SerializeField] CollectibleCoin coin;
    [SerializeField] int xpWhenKilled = 10;

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

    List<Hero> noticedHeroes = new List<Hero>();

    bool wasAttacking = false;

    Color expTextColor;

    bool frozen;

    RectTransform inGameUIIndicator;

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

        frozen = false;
        animator.speed = 1;

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

        expTextColor = expGainedText.color;

        isInit = true;

        if(inGameUIIndicator == null) inGameUIIndicator = R.get.ui.menuIngame.SpawnIndicator(UIPositionIndicatorPrefab);
        else
        {
            inGameUIIndicator.transform.localScale = Vector3.zero;
            inGameUIIndicator.DOScale(Vector3.one, 0.5f);
        }
        
    }

    protected override void Update()
    {

        base.Update();

        if (dead || !R.get.game.isOn || !isInit) return;

        if (!frozen && Vector3.Distance(previousPos, transform.position) < 0.1f) //a set value of "min movement"
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

        IndicatePositionOnUI();

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
                    if (wasAttacking)
                    {

                        animator.SetTrigger(movementString);
                    }
                    
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

        if (attackCounter < attackEveryXSeconds || !R.get.game.isOn) return;

        wasAttacking = true;
        
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
        if (Vector3.Distance(R.get.levelManager.level.payload.GetComponent<Collider>().ClosestPoint(transform.position), transform.position) < attackRange) R.get.levelManager.level.payload.TakeDamage(1);

    }

    public void Freeze(float duration)
    {
        StartCoroutine(FreezeCoroutine(duration));
    }

    IEnumerator FreezeCoroutine(float duration)
    {
        frozen = true;
        agent.enabled = false;
        animator.speed = 0;

        float t = 0;
        while(t < duration && !dead)
        {
            t += Time.deltaTime;
            yield return null;
        }

        if(!dead)
        {
            frozen = false;
            animator.speed = 1;
            agent.enabled = true;
        }
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
        target = R.get.levelManager.level.payload.GetComponent<Collider>().ClosestPoint(transform.position);

        Collider[] results = Physics.OverlapSphere(transform.position, visionDistance, heroes, QueryTriggerInteraction.Collide);
        if (results != null && results.Length > 0)

        {
            for (int i = 0; i < results.Length; i++)
            {
                if (!results[i].GetComponent<Hero>().dead
                && noticedHeroes.Contains(results[i].GetComponent<Hero>())
                && !Physics.Raycast(transform.position + Vector3.up * 0.2f, results[i].transform.position - transform.position, visionDistance * transform.lossyScale.z, obstacles, QueryTriggerInteraction.Collide)
                && Vector3.Distance(results[i].transform.position, transform.position) < Vector3.Distance(target, transform.position))
                {
                    target = results[i].transform.position;
                }
            }

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

        speedMultiplier = 1; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (R.get.game.isOn && !dead || !isInit)
        {
            if (other.CompareTag("Bullet"))
            {
                //Debug.Log("touché");
                Bullet bullet = other.GetComponent<Bullet>();
                TakeDamage(bullet.damage, bullet.transform.forward * bullet.baseProjectionForce);
                if (!noticedHeroes.Contains(bullet.shooter)) noticedHeroes.Add(bullet.shooter);

                if (bullet.setsOnFireCounter > 0)
                {
                    onFireCounter = bullet.setsOnFireCounter;
                }
                else
                {
                    Instantiate(hitFx, bullet.transform.position, default).gameObject.SetActive(true);
                }

                if(!bullet.goesThroughEnemies) bullet.Kill();
            }

        }

    }

    protected override void Die(Vector3 ragdollForce)
    {
        base.Die(ragdollForce);

        expGainedText.color = expTextColor;

        expGainedText.text =  "+" + R.get.game.GetXP(xpWhenKilled) + "xp";
        expGainedText.gameObject.SetActive(true);
        expGainedText.transform.forward = R.get.mainCamera.transform.forward;
        expGainedText.transform.localScale = Vector3.zero;
        expGainedText.transform.DOScale(Vector3.one, 0.3f);
        expGainedText.transform.DOMove(expGainedText.transform.position + Vector3.up, 1f).SetEase(Ease.OutSine).OnComplete(() => expGainedText.gameObject.SetActive(false));
        expGainedText.DOColor(new Color(0, 0, 0, 0), 0.5f).SetDelay(0.5f);

        inGameUIIndicator.transform.DOScale(Vector3.zero, 0.5f);

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

        expGainedText.gameObject.SetActive(false);
        
        R.get.levelManager.level.enemiesPool.Add(this);
        gameObject.SetActive(false);


    }

    public void IndicatePositionOnUI()
    {
        if(R.get.game.CheckIfEnemyIsInView(transform.position))
        {
            inGameUIIndicator.gameObject.SetActive(false);
        }

        else
        {
            inGameUIIndicator.anchoredPosition = R.get.ui.menuIngame.GetIndicatorPos(transform.position);
        }
        
    }

    private void OnDrawGizmosSelected()
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