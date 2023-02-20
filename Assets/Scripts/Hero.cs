using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine.AI;

public class Hero : Character
{

    [FoldoutGroup("Debug")] public Transform targetGO;

    [FoldoutGroup("Refs")] public DrawnLine line;
    [FoldoutGroup("Refs")] public Shapes.Disc visionCircle; //temp
    [FoldoutGroup("Refs")] public Transform aim;
    [FoldoutGroup("Refs")] public TextMeshPro levelText;
    [FoldoutGroup("Refs")] public TextMeshPro countdown;

    [FoldoutGroup("Refs"), SerializeField] private ParticleSystem heal;
    [FoldoutGroup("Refs"), SerializeField] private ParticleSystem shield;
    [FoldoutGroup("Refs"), SerializeField] private ParticleSystem boost;
    [FoldoutGroup("Refs"), SerializeField] private ParticleSystem levelUp;
    [FoldoutGroup("Refs"), SerializeField] public Sprite icon;




    [SerializeField] float rigAimOffsetDegrees = 47;

    [SerializeField] private bool canShootWhileWalking;
    [SerializeField] public Color lineColor;
    [SerializeField] public float insideCircleTransparency = 200;
    [SerializeField] float lineThickness = 0.2f;
    List<Vector3> path;
    Coroutine movingOnPathCoroutine;
    [SerializeField, OnValueChanged("SetUpCircle")] float visionRay = 5f;
    Vector3 nextGoal;
    [SerializeField] LayerMask ennemisLayerMask;
    [SerializeField] LayerMask alliesLayerMask;
    [SerializeField] LayerMask canShootLayerMask;
    [SerializeField] LayerMask destructiblesLayerMask;

    [SerializeField] bool slowsDown; //effect ray is visionRay
    [SerializeField, ShowIf("slowsDown")] float slowDownMultiplier = 0.5f;

    [SerializeField] bool heals;

    [SerializeField, ShowIf("heals")] float healsEveryXSeconds = 3f;

    [SerializeField] bool boosts;
    [SerializeField, ShowIf("boosts")] float boostMultiplier = 2f;

    [SerializeField] bool shootsConstantly;
    [SerializeField] float maxDegTurnPerFrame = 12f;

    [SerializeField] float speedMultiplierWhenShooting = 0.5f;

    [SerializeField] public string heroName;
    [SerializeField, TextArea(6,10)] public string description;


    bool pauseOnPath;
    bool forceMovement = false;

    bool canShoot = true;

    bool isShooting = false;

    Coroutine crateBonusCoroutine;

    Vector3 interestPoint;

    Collider previousTarget;

    public bool inCinematic;

    float initLampIntensity;

    bool isInvulnerable = false;


    float levelSpeedMultiplier;

    int level = 1;

    Shapes.Disc secondaryVisionCircle;



    public override void Init()
    {

        base.Init();

        initLampIntensity = gun.lamp.intensity;
        gun.lamp.intensity = 0;

        animator.Play("Idle");
        animator.SetFloat("WalkingSpeed", speed / 3f); //the 3f is pretty random,  value should be adjusted

        path = new List<Vector3>();

        line.transform.SetParent(null);
        line.transform.position = Vector3.zero + Vector3.up * 0.1f;
        line.transform.localRotation = Quaternion.Euler(Vector3.zero);
        line.transform.localScale = Vector3.one;

        line.SetColor(lineColor);
        line.SetThickness(lineThickness);

        nextGoal = transform.position;

        //lifeBar.color = lineColor;
        lifeBar.fillAmount = 1;

        SetUpVisionCircle();

        if (gun != null)
        {
            if (gun.data.bulletSpeed != 0) gun.data.bulletLifetime = visionRay / gun.data.bulletSpeed;
            gun.data.ennemiesLayerMask = ennemisLayerMask;


        }

        level = 1;
        levelText.text = "lvl " + level.ToString();

        GetValuesFromLevel();

        visionCircle.transform.SetParent(null);

        if (heals) StartCoroutine(HealAroundCoroutine());

        if (canShootWhileWalking) canShoot = true;
    }

    void GetValuesFromLevel()
    {
        if (gun != null)
        {

            gun.data.bulletsPerSecond *= R.get.levelDesign.EvaluateCharaFirerateMultiplier(PlayerPrefs.GetInt("Level" + heroName));
            gun.data.damagePerBullet *= R.get.levelDesign.EvaluateCharaDamageMultiplier(PlayerPrefs.GetInt("Level" + heroName));
        }

        levelSpeedMultiplier = R.get.levelDesign.EvaluateCharaSpeedMultiplier(PlayerPrefs.GetInt("Level" + heroName));
    }


    public void LevelUp()
    {
        level++;
        levelText.text = "lvl " + level.ToString();
        GetValuesFromLevel();
        levelUp.Play();
    }
    void SetUpVisionCircle()
    {
        visionCircle.Radius = visionRay;
        visionCircle.transform.GetChild(0).GetComponent<Shapes.Disc>().Radius = visionRay;
        visionCircle.Color = lineColor;


        if (gun.isGrenade)
        {

            visionCircle.transform.GetChild(0).GetComponent<Shapes.Disc>().Color =  new Color(0, 0, 0, 0);

            secondaryVisionCircle = Instantiate(visionCircle, visionCircle.transform);
            secondaryVisionCircle.gameObject.SetActive(true);
            secondaryVisionCircle.transform.localRotation = Quaternion.Euler(Vector3.zero);
            secondaryVisionCircle.transform.localPosition = Vector3.zero;
            secondaryVisionCircle.Radius = visionRay + startGunData.explosionRay * 0.5f;
            secondaryVisionCircle.Thickness = startGunData.explosionRay;
            Color insideColor = lineColor;
            insideColor.a = insideCircleTransparency;
            secondaryVisionCircle.Color = insideColor;
        }
        else
        {
            Color insideColor = lineColor;
            insideColor.a = insideCircleTransparency;
            visionCircle.transform.GetChild(0).GetComponent<Shapes.Disc>().Color = insideColor;
        }

    }
    public void SetPath(List<Vector3> points)
    {
        path.Clear();

        path.AddRange(points);
        visionCircle.gameObject.SetActive(false);
        line.Init();
        line.SetPoints(points);
        if (movingOnPathCoroutine != null) StopCoroutine(movingOnPathCoroutine);
        movingOnPathCoroutine = StartCoroutine(MoveOnPath());

        if (pauseOnPath) forceMovement = true;

    }

    IEnumerator MoveOnPath()
    {
        animator.SetTrigger("Run");
        
        line.gameObject.SetActive(true);
        float speed = 2f;
        canShoot = canShootWhileWalking;

        bool stop = false;
        while (!dead && path.Count > 0 && !stop)
        {
            float t = 0;
            while (!dead && path.Count > 0 && Vector3.Distance(path[0], transform.position) > 0.01f && !stop) 
            {
                Vector3 previousPos = transform.position;

                MoveTowards(path[0],  1);

                if (Vector3.Distance(R.get.levelManager.level.payload.GetComponent<Collider>().ClosestPoint(transform.position), transform.position) < (col as CapsuleCollider).radius)
                {
                    transform.position = previousPos;
                    stop = true;
                    break;
                }

                nextGoal = path[path.Count > 1  ? 1 : 0];

                /*// rotation
                Debug.Log("path 0: " + path[0].ToString() + " ---- transform.position : " + transform.position.ToString());
                if(path.Count> 4)
                {
                    //find the vector pointing from our position to the target
                    Vector3 _direction = (path[3] - transform.position).normalized;

                    //create the rotation we need to be in to look at the target
                    Quaternion _lookRotation = Quaternion.LookRotation(_direction);

                    //rotate us over time according to speed until we are in the required rotation
                    transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 90f);

                    //transform.Rotate(0,5,0);

                    //transform.Rotate(0, 5, 0);
                }*/



                t += Time.deltaTime * speed;

                line.MoveFirstPoint(transform.position);

                if (pauseOnPath)
                {
                    canShoot = true;
                    SetIdle();
                    yield return new WaitWhile(() => pauseOnPath && !forceMovement);
                    canShoot = canShootWhileWalking;
                    animator.SetTrigger("Run");
                }
                else yield return null;
            }

            if (dead) break;
            if(path.Count > 0) path.RemoveAt(0);
            line.RemovePoint(0);

        }
        canShoot = true;
        StopMoving();
        SetIdle();
        line.gameObject.SetActive(false);

        forceMovement = false;
    }

    protected override void MoveTowards(Vector3 goal, float tempSpeedModifier = 1f)
    {
        base.MoveTowards(goal, tempSpeedModifier * levelSpeedMultiplier);

    }

    protected override void Update()
    {
        base.Update();
        if (dead || !R.get.game.isOn || inCinematic) return;

        if (slowsDown)
            SlowAround();

        if (boosts) BoostAround();

        levelText.transform.forward = R.get.mainCamera.transform.forward;

        TurnTowardsClosestInterestPoint();
    }



    public void Heal(int amount)
    {
        heal.Play();
        PVs = Mathf.Clamp(PVs + amount, 0, maxPVs);
        UpdateLifeBar();
    }

    public void TeleportTo(Vector3 point)
    {

        StopMoving();
        ResetPath();
        agent.Warp(point);


    }

    IEnumerator HealAroundCoroutine()
    {
        while (!dead)
        {
            Collider[] results = Physics.OverlapSphere(transform.position, visionRay, alliesLayerMask);
            foreach (Collider collider in results)
            {
                collider.GetComponent<Hero>().Heal(1);
            }
            yield return new WaitForSeconds(healsEveryXSeconds);
        }


    }
    void SlowAround()
    {
        Collider[] results = Physics.OverlapSphere(transform.position, visionRay, ennemisLayerMask);
        foreach (Collider collider in results)
        {
            collider.GetComponentInParent<Enemy>().SlowDown(slowDownMultiplier);
        }

    }

    void BoostAround()
    {
        Collider[] results = Physics.OverlapSphere(transform.position, visionRay, alliesLayerMask);
        foreach (Collider collider in results)
        {
            collider.GetComponent<Hero>().BoostGunDamage(boostMultiplier);
        }

    }

    public override void StopMoving()
    {
        base.StopMoving();

        if (movingOnPathCoroutine != null) StopCoroutine(movingOnPathCoroutine);
        line.gameObject.SetActive(false);
        visionCircle.gameObject.SetActive(false);
    }

    public void BoostGunDamage(float multiplier)
    {
        if(gun != null) gun.damageMultiplier = multiplier;
    }


    protected override void Die(Vector3 ragdollForce)
    {
        base.Die(Vector3.zero);


        path.Clear();
        visionCircle.gameObject.SetActive(false);
        line.gameObject.SetActive(false);

        gun.lamp.gameObject.SetActive(false);

        int heroCount = 0;
        foreach(Hero hero in R.get.game.heroes)
        {
            if (!hero.dead) heroCount++;
        }

        if (heroCount == 0) R.get.game.Lose();

        StartCoroutine(WaitForResurrectionCoroutine(10));
    }


    public void Revive()
    {

        PVs = maxPVs / 2f;

        lifeBar.transform.parent.gameObject.SetActive(true);

        countdown.gameObject.SetActive(false);

        path.Clear();

        UpdateLifeBar();
        gun.lamp.gameObject.SetActive(true);

        dead = false;
        
        DeactivateRagdoll();
    }



    public override void TakeDamage(float amount, Vector3 ragdollForce)
    {
        if (isInvulnerable) return;

        base.TakeDamage(amount, ragdollForce);
        animator.SetTrigger("Hit");
        DisableRigSuppTemporarily(0.5f);

    }


    void TurnTowardsClosestInterestPoint()
    {

        Vector3 previousInterestPoint = interestPoint;

        interestPoint = nextGoal == transform.position ? transform.position + transform.forward : transform.position + (nextGoal - transform.position).normalized;
        bool noTarget = true;


        if (gun != null)
        {
            Collider interestCollider = null;
            bool previousColliderIn = false;

            float currentDistance = gun.isGrenade ? 0 : visionRay * 2f;

            bool thereIsAnEnemyInSight = false;
            Collider[] results = Physics.OverlapSphere(transform.position, visionRay, ennemisLayerMask, QueryTriggerInteraction.Collide);
            if (results.Length > 0)
            {
                float distanceToCollider;
                bool enemyIsCrawling = false;
                foreach (Collider collider in results)
                {
                    distanceToCollider = Vector3.Distance(collider.transform.position, transform.position);
                    Vector3 direction = collider.transform.position - gun.transform.position;
                    //direction.y = 0;
                    if (collider.TryGetComponent(out Enemy enemy) && R.get.game.CheckIfEnemyIsInView(enemy.transform.position) && (gun.isGrenade ? distanceToCollider > currentDistance : distanceToCollider < currentDistance) && !enemy.dead && !Physics.Raycast(gun.transform.position, direction.normalized, distanceToCollider, obstacles, QueryTriggerInteraction.Collide))
                    {
                        if (collider == previousTarget) previousColliderIn = true;
                        interestCollider = collider;
                        interestPoint = collider.transform.position;
                        currentDistance = Vector3.Distance(interestPoint, transform.position);
                        noTarget = false;
                        thereIsAnEnemyInSight = true;
                        if (enemy.isCrawling) enemyIsCrawling = true;
                        else enemyIsCrawling = false;
                    }

                }

                interestPoint += (enemyIsCrawling ? Vector3.up * 1f : Vector3.up * 2f);

            }

            results = Physics.OverlapSphere(transform.position, visionRay, destructiblesLayerMask, QueryTriggerInteraction.Collide);
            if (results.Length > 0)
            {
                float distanceToCollider;

                foreach (Collider collider in results)
                {
                    distanceToCollider = Vector3.Distance(collider.transform.position, transform.position);
                    Vector3 direction = collider.transform.position - gun.transform.position;
                    //direction.y = 0;
                    if (collider.TryGetComponent(out Destructible destructible) && R.get.game.CheckIfEnemyIsInView(destructible.transform.position) && !Physics.Raycast(gun.transform.position, direction.normalized, distanceToCollider, obstacles, QueryTriggerInteraction.Collide))
                    {
                        if (destructible.explodes || (!thereIsAnEnemyInSight && (gun.isGrenade ? distanceToCollider > currentDistance : distanceToCollider < currentDistance)))
                        {
                            if (collider == previousTarget) previousColliderIn = true;
                            interestCollider = collider;
                            interestPoint = collider.transform.position + Vector3.up * 0.75f;
                            currentDistance = Vector3.Distance(interestPoint, transform.position);
                            noTarget = false;

                        }

                    }

                }

            }


            //avoids "flickering" of rotation when two targets are about the same distance by favoring the one it was targeting previously even if it's a little further
            if (!gun.isGrenade && !noTarget && interestCollider != previousTarget && previousColliderIn)
            {
                Debug.Log("Anti flickering");
                interestCollider = previousTarget;
                interestPoint = previousTarget.transform.position;
                interestPoint.y = previousInterestPoint.y;
                noTarget = false;
            }
                previousTarget = interestCollider;


                isShooting = !noTarget;

            //pauseOnPath = !noTarget;

            //Vector3 dir = (interestPoint - transform.position).normalized;
            Vector3 dir = (interestPoint - gun.bulletSource.position).normalized;
            Vector3 horizontalDir = dir;
            horizontalDir.y = 0;


            if (!noTarget)
            {
                bool enemyTooClose = false;

                Vector3 fromTransformToBulletSource = gun.bulletSource.position - transform.position;
                Vector3 fromTransformToInterestPoint = interestCollider.ClosestPoint(transform.position) - transform.position;

                //flatten em
                fromTransformToBulletSource.y = 0;
                fromTransformToInterestPoint.y = 0;


                if (fromTransformToBulletSource.magnitude > fromTransformToInterestPoint.magnitude) enemyTooClose = true;


                //the shooting part
                bool shot = false;
                if (gun != null)
                {

                    //if there is a target they can shoot, player will try to shoot (which will only happen if there is a bullet ready)
                    if (shootsConstantly || (!enemyTooClose && canShoot && !noTarget && (Physics.Raycast(gun.bulletSource.position, gun.bulletSource.forward - gun.bulletSource.forward.y * Vector3.up, visionRay, canShootLayerMask))))
                    {
                        gun.lamp.DOKill();
                        gun.lamp.DOIntensity(initLampIntensity, 0.3f);
                        //inflict damage and triggers the firing animation
                        if (gun.TryShoot())
                        {
                            shot = true;
                            animator.SetTrigger("Fire");
                        }
                    }
                    else if (canShoot && !noTarget && enemyTooClose)
                    {

                        //inflict damage and triggers the firing animation
                        if (gun.TryShoot(true))
                        {
                            shot = false;
                            animator.SetTrigger("Fire");
                        }
                        

                    }
                }


                if(enemyTooClose)
                {
                    Debug.Log("Enemy too close, fixing forward");
                    dir = (interestPoint - transform.position).normalized;
                    dir.y = 0;
                    horizontalDir = dir;

                }

                //the turning part
                targetGO.transform.position = interestPoint;


                if(!shot) animator.SetTrigger("Aim");


                //turns the aim towards the target, and the legs follow if there is a big enough rotation to avoid breaking the poor guy's spine

                if (Vector3.Angle(horizontalDir.normalized, transform.forward) > 60) transform.forward = Vector3.Lerp(transform.forward, Vector3.RotateTowards(transform.forward, horizontalDir.normalized, Mathf.Deg2Rad * maxDegTurnPerFrame, default), 0.8f);
                else aim.forward = Vector3.Lerp(aim.forward, Vector3.RotateTowards(aim.forward, Quaternion.Euler(0, rigAimOffsetDegrees, 0) * dir.normalized, Mathf.Deg2Rad * maxDegTurnPerFrame, default), 0.8f);
            }
            else
            {
                //aim.rotation = transform.rotation * Quaternion.Euler(0, 33, 0);

                animator.ResetTrigger("Aim");
                animator.SetTrigger("Down");
                gun.lamp.DOKill();
                gun.lamp.DOIntensity(0, 0.3f);

                horizontalDir = (interestPoint - transform.position).normalized;
                horizontalDir.y = 0;

                //turns the forward of both the transform and the aim towards the next point on the path

                if (path != null && path.Count > 0)
                {
                    transform.forward = Vector3.Lerp(transform.forward, Vector3.RotateTowards(transform.forward, new Vector3(horizontalDir.x, 0, horizontalDir.z).normalized, Mathf.Deg2Rad * maxDegTurnPerFrame, default), 0.8f);
                    aim.forward = Vector3.Lerp(aim.forward, Vector3.RotateTowards(aim.forward, Quaternion.Euler(0, rigAimOffsetDegrees, 0) * horizontalDir.normalized, Mathf.Deg2Rad * maxDegTurnPerFrame, default), 0.8f);

                }
            }

            
        }


    }

        public void GoToPointWithAgent(Vector3 point, float delay, float margin = 0.25f)
        {

            StartCoroutine(GoToPointCoroutine(point, delay, margin));
        }

        IEnumerator GoToPointCoroutine(Vector3 point, float delay, float margin = 0.25f)
        {

            inCinematic = true;

            animator.SetTrigger("Down");
            gun.lamp.DOKill();
            gun.lamp.DOIntensity(0, 0.3f);

            if (delay > 0) yield return new WaitForSeconds(delay);



            col.enabled = false;
            Vector3 safePos = transform.position;

            ResetPath();
            Collider[] results = Physics.OverlapCapsule(transform.position + (agent.radius + 0.1f) * Vector3.up, transform.position + (agent.radius + 0.1f) * 2f * Vector3.up, agent.radius * 2f, obstacles);
            if (results.Length > 0)
            {
                agent.enabled = false;
                Debug.Log("Was blocked by : " + results[0]);


                //tries to find a "safe" point by looking for safe points around it
                //otherwise, if the agent is enabled while it's considered "in" an object, the agent will ignore ALL obstacles which uh , sucks
                Vector3[] pointsToCheck = new Vector3[13];
                float angle = 0;


                for (int i = 0; i < 13; i++)
                {
                    float x = Mathf.Sin(Mathf.Deg2Rad * angle) * (agent.radius * 3f);
                    float y = Mathf.Cos(Mathf.Deg2Rad * angle) * (agent.radius * 3f);

                    pointsToCheck[i] = new Vector3(x, 0.1f, y) + transform.position;

                    angle += (380f / 11);
                }

                foreach (Vector3 pointAround in pointsToCheck)
                {
                    if (Physics.OverlapCapsule(pointAround + (agent.radius + 0.1f) * Vector3.up, pointAround + (agent.radius + 0.1f) * 3f * Vector3.up, agent.radius * 3f, obstacles).Length == 0)
                    {
                        safePos = pointAround;
                        break;
                    }
                }

                //agent.transform.position = safePos;
                NavMeshHit myNavHit;
                if (NavMesh.SamplePosition(safePos, out myNavHit, 5, -1))
                {
                    safePos = myNavHit.position;
                }

                if (safePos != transform.position)
                {
                    Debug.Log("safePos found : " + transform.position);
                    while (Vector3.Distance(transform.position, safePos) > 0.05f)
                    {
                        transform.forward = safePos - transform.position;
                        MoveTowards(safePos);
                        yield return null;
                    }

                }

            }


            yield return new WaitForSeconds(0.1f);

            agent.enabled = true;


            Debug.Log(heroName + " Owner : " + agent.navMeshOwner);
            Debug.Log(heroName + " IsOnNavmesh : " + agent.isOnNavMesh);

            //agent.transform.position = safePos;
            NavMeshHit navHitDestination;
            if (NavMesh.SamplePosition(point, out navHitDestination, 5, -1))
            {
                point = navHitDestination.position;
            }

            if (!agent.SetDestination(point)) Debug.Log("Failed smh");

            yield return new WaitWhile(() => agent.pathPending);

            //agent.SetDestination(point);

            animator.ResetTrigger("Idle");
            animator.SetTrigger("Run");
            animator.SetFloat("WalkingSpeed", 3);

            yield return new WaitWhile(() => Vector3.Distance(point, transform.position) >= margin);

            UpdateLifeBar();
            animator.SetFloat("WalkingSpeed", speed / 3f);

            col.enabled = true;

            agent.enabled = (false);
            inCinematic = false;
            animator.SetTrigger("Idle");

        }
    
        public void ResetPath()
        {

            if (path != null) path.Clear();
            line.gameObject.SetActive(false);
        }

        private void DisableRigSuppTemporarily(float time)
        {
            StartCoroutine(DisableRigSuppCoroutine(time));
        }

        IEnumerator DisableRigSuppCoroutine(float time)
        {
            UnityEngine.Animations.Rigging.Rig rig = aim.parent.GetComponent<UnityEngine.Animations.Rigging.Rig>();
            rig.weight = 0;
            yield return new WaitForSeconds(time * 0.75f);
            float t = 0;
            while (t < time * 0.25)
            {
                rig.weight = t / (time * 0.25f);
                t += Time.deltaTime;
                yield return null;
            }
            rig.weight = 1;
        }


        public void GetBoosterCrate(float firerateMultiplier, bool turnsInvulnerable, float bonusDuration)
        {
        if (dead) return;
        
            if (crateBonusCoroutine != null) StopCoroutine(crateBonusCoroutine);

            ResetAllCrateBonuses();

            crateBonusCoroutine = StartCoroutine(CrateBonusCoroutine(firerateMultiplier, turnsInvulnerable, bonusDuration));
        }

        IEnumerator CrateBonusCoroutine(float firerateMultiplier, bool turnsInvulnerable, float bonusDuration)
        {

            gun.firerateMultiplier = firerateMultiplier;

            List<ParticleSystemRenderer> renderers = new List<ParticleSystemRenderer>();
            if (firerateMultiplier > 1)
            {
                boost.Play();
                renderers.AddRange(boost.GetComponentsInChildren<ParticleSystemRenderer>());
            }
            if (turnsInvulnerable)
            {
                shield.Play();
                renderers.AddRange(shield.GetComponentsInChildren<ParticleSystemRenderer>());
            }
            isInvulnerable = turnsInvulnerable;
            //probably enable an fx or something like that

            float t = 0;

            if (turnsInvulnerable || firerateMultiplier > 1)
            {

                float blinkDuration = 2f;
                float clignotementCounter = 0;

                //countDown.gameObject.SetActive(true);
                yield return new WaitForSeconds(Mathf.Max(0, bonusDuration - blinkDuration));


                while (t < 1)
                {

                    if (clignotementCounter > 0.2f)
                    {
                        clignotementCounter = 0;
                        if (renderers[0].enabled) foreach (ParticleSystemRenderer renderer in renderers) renderer.enabled = false;
                        else foreach (ParticleSystemRenderer renderer in renderers) renderer.enabled = true;

                    }
                    t += Time.deltaTime / blinkDuration;
                    clignotementCounter += Time.deltaTime;
                    yield return null;

                }
            }



            ResetAllCrateBonuses();
        }
    
    public void ResetAllCrateBonuses()
    {
        //countDown.gameObject.SetActive(false);
        isInvulnerable = false;
        gun.firerateMultiplier = 1;

        shield.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        heal.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        boost.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    IEnumerator WaitForResurrectionCoroutine(float duration = 10)
    {
        float t = 0;
        bool resurrected = false;
        countdown.gameObject.SetActive(true);
        countdown.transform.forward = R.get.mainCamera.transform.forward;
        while (t < duration && ! resurrected)
        {
            Collider[] results = Physics.OverlapSphere(transform.position, 2, alliesLayerMask, QueryTriggerInteraction.Collide);
            if (results.Length > 0)
            {
                Revive();
                resurrected = true;
            }
            countdown.text = Mathf.RoundToInt(duration - t).ToString();
            t += Time.deltaTime;
            yield return null;
        }
        if (!resurrected)
        {
            gameObject.SetActive(false);
        }
    }


#if UNITY_EDITOR

    [Button]
    //for debug purposes obvi
    public void Instakill()
    {
        TakeDamage(553153, Vector3.zero);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = lineColor;
        Gizmos.DrawLine(transform.position, interestPoint);
    }
#endif
}
