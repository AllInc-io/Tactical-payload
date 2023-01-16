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
    [FoldoutGroup("Refs")] public TextMeshPro countDown;

    [FoldoutGroup("Refs"), SerializeField] private ParticleSystem heal;
    [FoldoutGroup("Refs"), SerializeField] private ParticleSystem shield;
    [FoldoutGroup("Refs"), SerializeField] private ParticleSystem boost;
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

        lifeCircle.color = lineColor;
        lifeCircle.fillAmount = 0;

        SetUpCircle();

        if (gun != null)
        {
            if (gun.data.bulletSpeed != 0) gun.data.bulletLifetime = visionRay / gun.data.bulletSpeed;
            gun.data.ennemiesLayerMask = ennemisLayerMask;

            gun.data.bulletsPerSecond *= R.get.levelDesign.EvaluateCharaFirerateMultiplier(PlayerPrefs.GetInt("Level" + heroName));
            gun.data.damagePerBullet *= R.get.levelDesign.EvaluateCharaDamageMultiplier(PlayerPrefs.GetInt("Level" + heroName));
        }

        levelSpeedMultiplier = R.get.levelDesign.EvaluateCharaSpeedMultiplier(PlayerPrefs.GetInt("Level" + heroName));

        visionCircle.transform.SetParent(null);

        if (heals) StartCoroutine(HealAroundCoroutine());

        if (canShootWhileWalking) canShoot = true;
    }


    void SetUpCircle()
    {
        visionCircle.Radius = visionRay*1.2f;
        visionCircle.transform.GetChild(0).GetComponent<Shapes.Disc>().Radius = visionRay * 1.2f;
        visionCircle.Color = lineColor;

        Color insideColor = lineColor;
        insideColor.a = insideCircleTransparency;
        visionCircle.transform.GetChild(0).GetComponent<Shapes.Disc>().Color = insideColor;
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
        while (!dead && path.Count > 0)
        {
            float t = 0;
            while (!dead && path.Count > 0 && Vector3.Distance(path[0], transform.position) > 0.01f)
            {

                MoveTowards(path[0],  1);
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

        countDown.transform.forward = Vector3.forward + Vector3.down * 0.3f;

        TurnTowardsClosestInterestPoint();
    }



    public void Heal(int amount)
    {
        heal.Play();
        PVs = Mathf.Clamp(PVs + amount, 0, maxPVs);
        UpdateLifeCircle();
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


        interestPoint = nextGoal == transform.position ? transform.position + transform.forward : transform.position + (nextGoal - transform.position).normalized;
        bool noTarget = true;


        if (gun != null)
        {

            float currentDistance = gun.isGrenade ? 0 : visionRay * 2f;

            Collider[] results = Physics.OverlapSphere(transform.position, visionRay, ennemisLayerMask, QueryTriggerInteraction.Collide);
            if (results.Length > 0)
            {
                float distanceToCollider;
                Collider interestCollider = null;
                bool previousColliderIn = false;
                foreach (Collider collider in results)
                {
                    distanceToCollider = Vector3.Distance(collider.transform.position, transform.position);
                    Vector3 direction = collider.transform.position - gun.transform.position;
                    //direction.y = 0;
                    if (collider.TryGetComponent(out Enemy enemy) && (gun.isGrenade ? distanceToCollider > currentDistance : distanceToCollider < currentDistance) && !enemy.dead && !Physics.Raycast(gun.transform.position, direction.normalized, distanceToCollider, obstacles, QueryTriggerInteraction.Collide))
                    {
                        if (collider == previousTarget) previousColliderIn = true;
                        interestCollider = collider;
                        interestPoint = collider.transform.position;
                        currentDistance = Vector3.Distance(interestPoint, transform.position);
                        noTarget = false;
                    }

                }

                //avoids "flickering" of rotation when two targets are about the same distance by favoring the one it was targeting previously even if it's a little further
                if (!gun.isGrenade && !noTarget && interestCollider != previousTarget && previousColliderIn && Vector3.Distance(previousTarget.transform.position, transform.position) <= currentDistance * 2f)
                {
                    Debug.Log("Anti flickering");
                    interestCollider = previousTarget;
                    interestPoint = previousTarget.transform.position;

                }
                previousTarget = interestCollider;


                isShooting = !noTarget;
                //pauseOnPath = !noTarget;



            }
        }



        Vector3 dir = (interestPoint - transform.position).normalized;

        Vector3 horizontalDir = dir;
        horizontalDir.y = 0;

        if (!noTarget)
        {

            targetGO.transform.position = interestPoint;
            /*
               Vector3 _direction = (aim.position - transform.position).normalized;

               //create the rotation we need to be in to look at the target
               Quaternion _lookRotation = Quaternion.LookRotation(interestPoint);

               //rotate us over time according to speed until we are in the required rotation
               aim.rotation = Quaternion.Slerp(aim.rotation, _lookRotation, Time.deltaTime * 90f);

               */

            animator.SetTrigger("Aim");


            //turns the aim towards the target, and the legs follow if there is a big enough rotation to avoid breaking the poor guy's spine
            //Debug.Log(Vector3.SignedAngle(transform.forward, aim.forward, Vector3.up));

            if (Vector3.Angle(horizontalDir.normalized, transform.forward) > 60) transform.forward = Vector3.Lerp(transform.forward, Vector3.RotateTowards(transform.forward, horizontalDir.normalized, Mathf.Deg2Rad * maxDegTurnPerFrame, default), 0.8f);
            else aim.forward = Vector3.Lerp(aim.forward, Vector3.RotateTowards(aim.forward, Quaternion.Euler(0, rigAimOffsetDegrees, 0) * dir.normalized, Mathf.Deg2Rad * maxDegTurnPerFrame, default), 0.8f);
        }
        else
        {
            //aim.rotation = transform.rotation * Quaternion.Euler(0, 33, 0);
            animator.SetTrigger("Down");
            gun.lamp.DOKill();
            gun.lamp.DOIntensity(0, 0.3f);


            //turns the forward of both the transform and the aim towards the next point on the path

            if (path != null && path.Count > 0)
            {
                transform.forward = Vector3.Lerp(transform.forward, Vector3.RotateTowards(transform.forward, new Vector3(horizontalDir.x, 0, horizontalDir.z).normalized, Mathf.Deg2Rad * maxDegTurnPerFrame, default), 0.8f);
                aim.forward = Vector3.Lerp(aim.forward, Vector3.RotateTowards(aim.forward, Quaternion.Euler(0, rigAimOffsetDegrees, 0) * horizontalDir.normalized, Mathf.Deg2Rad * maxDegTurnPerFrame, default), 0.8f);

            }
        }

        if (gun != null)
        {

            //if there is a target they can shoot, player will try to shoot (which will only happen if there is a bullet ready)
            if (shootsConstantly || (canShoot && !noTarget && (Physics.Raycast(gun.bulletSource.position, gun.bulletSource.forward - gun.bulletSource.forward.y * Vector3.up, visionRay, ennemisLayerMask))))
            {
                gun.lamp.DOKill();
                gun.lamp.DOIntensity(initLampIntensity, 0.3f);
                //inflict damage and triggers the firing animation
                if (gun.TryShoot()) animator.SetTrigger("Fire");
            }
            else if (canShoot && !noTarget && Vector3.Distance(interestPoint, transform.position) < Vector3.Distance(transform.position, gun.bulletSource.position))
            {
                //inflict damage and triggers the firing animation
                if (Physics.Raycast(transform.position + Vector3.up * 0.5f, interestPoint - transform.position, out RaycastHit hit, gun.bulletRange, ennemisLayerMask)) if (gun.TryShoot(true)) animator.SetTrigger("Fire");

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
        if(results.Length > 0)
        {
            agent.enabled = false;
            Debug.Log("Was blocked by : " + results[0]);


            //tries to find a "safe" point by looking for safe points around it
            //otherwise, if the agent is enabled while it's considered "in" an object, the agent will ignore ALL obstacles which uh , sucks
            Vector3[] pointsToCheck = new Vector3[13];
            float angle = 0;

          
            for(int i= 0; i < 13; i++)
            {
                float x = Mathf.Sin(Mathf.Deg2Rad * angle) * (agent.radius * 3f);
                float y = Mathf.Cos(Mathf.Deg2Rad * angle) * (agent.radius * 3f);

                pointsToCheck[i] = new Vector3(x, 0.1f, y) + transform.position;

                angle += (380f / 11);
            }

            foreach(Vector3 pointAround in pointsToCheck)
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
                while(Vector3.Distance(transform.position, safePos) > 0.05f)
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

        yield return new WaitWhile (() => agent.pathPending) ;

        //agent.SetDestination(point);

        animator.ResetTrigger("Idle");
        animator.SetTrigger("Run");
        animator.SetFloat("WalkingSpeed", 3);

        yield return new WaitWhile(() => Vector3.Distance(point, transform.position) >= margin);

        UpdateLifeCircle();
        animator.SetFloat("WalkingSpeed", speed / 3f);

        col.enabled = true;

        agent.enabled = (false);
        inCinematic = false;
        animator.SetTrigger("Idle");

    }

    public void ResetPath()
    {

        if(path != null) path.Clear();
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
        while(t < time * 0.25)
        {
            rig.weight = t / (time * 0.25f);
            t += Time.deltaTime;
            yield return null;
        }
        rig.weight = 1;
    }

    
    public void GetBoosterCrate(float firerateMultiplier, bool turnsInvulnerable, float bonusDuration)
    {
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

        if(turnsInvulnerable || firerateMultiplier > 1)
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
                    Debug.Log("Blink");
                    if (renderers[0].enabled) foreach(ParticleSystemRenderer renderer in renderers) renderer.enabled = false;
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
        countDown.gameObject.SetActive(false);
        isInvulnerable = false;
        gun.firerateMultiplier = 1;

        shield.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        heal.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        boost.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;
        Gizmos.DrawLine(transform.position, interestPoint);
    }

}
