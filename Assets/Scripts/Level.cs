using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using System.Linq;
using PathCreation;

public class Level : MonoBehaviour
{

    public float length;
    public float width;


    [SerializeField] Wave[] waves;

    //[SerializeField] PathCreator path;

    public int totalEnemiesKilled;
    public int enemiesAlive;

    [SerializeField] public NavMeshSurface surface;

    [SerializeField] Transform[] oneRoadBlocks;

    [SerializeField] Destructible[] possibleDestructibles;

    [SerializeField, Range(0,1)] float destructiblesDensity;

    public List<Enemy> enemiesPool;

    public Payload payload;

    public bool pauseWaves;

    int enemiesToSpawn;

    public int totalPathLength;

    public float nextLevelSpawn = -25f;

    public Vector3 cameraPos;

    public LayerMask ground;

    public void Init()
    {



        
        float dayValue = Random.value;


        //randomize weather
        TimeOfDay timeOfDay = TimeOfDay.NIGHT;
        if (dayValue < 0.33f) timeOfDay = TimeOfDay.DAWN;
        else if (dayValue < 0.66f) timeOfDay = TimeOfDay.DAY;
        
        bool fog = Random.value < 0.3f;
        bool rain = Random.value < 0.3f && !fog;

        //set weather for tutorial level
        if (R.get.lastLevelFinished == 0)
        {
            timeOfDay = TimeOfDay.NIGHT;
            fog = false;
            rain = false;
        }

        //apply weather to level : TODO

        SpawnNewBlockInFront();

        surface.BuildNavMesh();

        //zones[0].SetUpCamera();
        payload.Init();


        StartCoroutine(EnemyWavesCoroutine());

        
    }


    public void StartLevel()
    {

    }


    public void Update()
    {
        //if(isOn) MoveCamera();
        SetUpCamera();
        
    }

    bool stop = false;

    IEnumerator EnemyWavesCoroutine()
    {
        int i = 0;


        yield return new WaitWhile(() => enemiesAlive > 0 || pauseWaves);

        while (!stop)
        {

            /*foreach(int entry in waveEntrances)
            {
                Vector3 entrance = enemySpawns[entry].position;
                Vector3 dir = (entrance).normalized;
                StartCoroutine(EnemyWave(1, entrance));
                R.get.ui.menuIngame.IndicateWaveIncoming(dir);

            }

            yield return new WaitForSeconds(3);
            */

            int progress = Mathf.RoundToInt(cameraPos.z / 3f);
            int totalZombiesAmount = Random.Range(R.get.levelDesign.EvaluateWaveAmount(progress, waves[i % waves.Length].zombieMinAmount), R.get.levelDesign.EvaluateWaveAmount(progress, waves[i % waves.Length].zombieMaxAmount));



            int maxSlope = 45;
            Vector3 entrance = payload.transform.position + Vector3.right * 10f * Random.Range(-1, 1);

            Ray ray = R.get.mainCamera.ViewportPointToRay(new Vector2(0.5f, 1.25f));

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground, QueryTriggerInteraction.UseGlobal) && Vector3.Angle(Vector3.up, hit.normal) < maxSlope) entrance.z = hit.point.z;
            else entrance.z = payload.transform.position.z + 20;

            Vector3 dir = (entrance).normalized;
                R.get.ui.menuIngame.IndicateWaveIncoming(dir, i + 1, waves.Length);

                StartCoroutine(EnemyWaveCoroutine(waves[i % waves.Length], totalZombiesAmount, entrance, totalZombiesAmount / 10f));


            float t = 0;


            while ((t < waves[i % waves.Length].secondsUntilNext && enemiesAlive > 0) || pauseWaves)
            {
                yield return null;
                t += Time.deltaTime;
            }

            i++;

            R.get.haptics.Haptic(HapticForce.Heavy);


        }


        //R.get.levelManager.level.ChangeZone();

    }


    private IEnumerator EnemyWaveCoroutine(Wave wave, int amount, Vector3 from, float waveSpawningDuration = 1f)
    {
        R.get.haptics.Haptic(HapticForce.Medium);
        enemiesToSpawn += amount;
        Vector3[] points = U.CreatePointsAroundCenter(from, 3, amount);
        float totalPercentage = 0;
        for (int i = 0; i < amount; i++)
        {
            totalPercentage += ((float)i / amount) * 100f;
            float percentageChecked = 0;

            int j = 0;

            while (j < wave.enemiesPercentage.Length - 1 && totalPercentage > wave.enemiesPercentage[j])
            {
                percentageChecked += wave.enemiesPercentage[j];
                j++;
            }

            //checks that the enemy isnt in an obstacle before spawning it to avoid weird navmesh behavior
            LayerMask mask = wave.enemiesPrefabs[j].obstacles;
            if (Physics.OverlapSphere(points[i] + Vector3.up * 0.5f, 0.5f, mask).Length == 0)
            {
                Enemy enemy = SpawnEnemy(wave.enemiesPrefabs[j]);
                enemy.transform.SetParent(transform.Find("EnemiesParent"));
                enemy.transform.position = points[i];

                enemy.Init();
            }
            else Debug.Log("Enemy not spawned because it was inside an obstacle at " + points[i]);
            enemiesToSpawn--;
            yield return new WaitForSeconds((float)(i * waveSpawningDuration) / amount);
        }

    }





    public Enemy SpawnEnemy(Enemy desiredPrefab)
    {

        enemiesAlive++;

        //will take an enemy from the pool if there is one, otherwise will instantiate one
        IEnumerable<Enemy> results = enemiesPool.Where(zombie => zombie.enemyName.Equals(desiredPrefab.enemyName));
        if (results.Count() > 0)
        { 
            Enemy zombie = results.FirstOrDefault();
            zombie.gameObject.SetActive(true);
            enemiesPool.Remove(zombie);
            return zombie;
        }
        else
        {
            Enemy zombie = Instantiate(desiredPrefab);
            return zombie;
        }

    }


    public void SetUpCamera()
    {
        
        cameraPos = Vector3.zero;
        int amount = 0;
        foreach (Hero hero in R.get.game.heroes)
        {
            if (!hero.dead)
            {
                cameraPos += hero.transform.position;
                amount++;
            }
        }

        if (amount == 0) return;

        cameraPos /= amount;

        R.get.mainCamera.transform.position = cameraPos + Vector3.up * 60 - Vector3.forward * 30f; //TEMP

        if(cameraPos.z >= nextLevelSpawn - 25f)
        {
            SpawnNewBlockInFront();
        }
    }

    void SpawnNewBlockInFront()
    {

        Transform newBlock = Instantiate(oneRoadBlocks[Random.Range(0, oneRoadBlocks.Length)], transform);
        newBlock.transform.position = Vector3.forward * (nextLevelSpawn + 25f);
        surface.BuildNavMesh();

        int length = 50;
        int width = 30;

        int step = 2;

        Vector3 startPos = new Vector3(-width/2f, 0, (nextLevelSpawn));

        //spawn crates
        for (int x = 0; x < width; x+= step)
        {
            for(int z = 0; z < length; z+= step)
            {
                float value = Random.value;
                if (value < destructiblesDensity) Instantiate(possibleDestructibles[0], new Vector3(x + startPos.x, 0.5f, z + startPos.z), default);
            }

        }


        nextLevelSpawn += 50f;

    }

    public void PauseWaves(float duration)
    {
        StartCoroutine(PauseWavesCoroutine(duration));
    }

    IEnumerator PauseWavesCoroutine(float duration)
    {
        pauseWaves = true;
        yield return new WaitForSeconds(duration);
        pauseWaves = false;
    }


}






[System.Serializable]
public struct Wave
{
    public int zombieMinAmount;
    public int zombieMaxAmount;
    public float totalEntrancesPercentage;
    public float secondsUntilNext;
    public Enemy[] enemiesPrefabs;
    public float[] enemiesPercentage;
}

[System.Serializable]
public struct WaveDataForOneZone
{
    public Wave[] waves;
}

public enum TimeOfDay
{ 
    DAY, 
    DAWN, 
    NIGHT
}

