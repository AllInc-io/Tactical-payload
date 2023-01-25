using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using TMPro;

public class Payload : MonoBehaviour
{

    //PathCreator path;
    [SerializeField] LayerMask canStopLayermask;
    [SerializeField] float speed = 0.05f;
    [SerializeField] TextMeshPro lifeText;
    [SerializeField] ParticleSystem explosionFX;

    float t = 0;

    bool isInit;
    bool stop = false;

    [SerializeField] int maxPvs;
    [HideInInspector] public int PVs;

    [SerializeField] Vector3 boxSize;
    [SerializeField] Vector3 boxOffset;

    [HideInInspector] public float startZOffset;

    public void Init()
    {



        maxPvs = R.get.levelDesign.GetPayloadLife(PlayerPrefs.GetInt("PayloadLifeLevel"));
        speed = R.get.levelDesign.GetPayloadSpeed(PlayerPrefs.GetInt("PayloadSpeedLevel"));

        PVs = maxPvs;
        lifeText.text = PVs.ToString();

        startZOffset = transform.position.z;

        StartCoroutine(UpdateProgressionCoroutine());

        isInit = true;
    }


    public void Update()
    {
        if (!isInit) return;

        bool canMove = true;

        if (Physics.OverlapBox(transform.position + boxOffset, boxSize, default, canStopLayermask).Length > 0) canMove = false;
        //casts in front to check that it can move;
        if (canMove && !stop) MoveForwardOnPath();

        R.get.ui.menuIngame.IndicatePayload(transform.position);

    }

    public void MoveForwardOnPath()
    {
        /*t += Time.deltaTime * speed/ path.path.length;

        Vector3 point = path.path.GetPointAtTime(t, EndOfPathInstruction.Stop);*/

        transform.position += transform.forward * Time.deltaTime * speed;

        
    }

    public void TakeDamage(int amount)
    {
        PVs -= amount;
        lifeText.text = PVs.ToString();
        if (PVs <= 0)
        {
            R.get.game.Lose();
            Explode();
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position + boxOffset, boxSize * 2f);
    }

    public void SetFaster(float multiplier)
    {
        speed *= multiplier;
    }

    public void SetMorePVs(float multiplier)
    {
        maxPvs = Mathf.RoundToInt(maxPvs * multiplier);
        PVs += Mathf.RoundToInt(maxPvs * (multiplier - 1));

        lifeText.text = PVs.ToString();
    }

    public void Explode()
    {
        Instantiate(explosionFX, transform.position, default);
        gameObject.SetActive(false);
    }

    IEnumerator UpdateProgressionCoroutine()
    {
        bool stop = false;
        bool passingLandmark = false;
        int z = 0;
        while(!stop && PVs > 0)
        {
            z = Mathf.RoundToInt(transform.position.z - startZOffset);

            passingLandmark = false;

            if (z == R.get.levelManager.level.nextProgressionLandmark)
            {
                passingLandmark = true;
                R.get.levelManager.level.PassLandmark();
            }
            R.get.ui.menuIngame.IndicateProgression(Mathf.RoundToInt(transform.position.z - startZOffset), passingLandmark);



            yield return new WaitWhile(() => transform.position.z == z);
        }
    }
}
