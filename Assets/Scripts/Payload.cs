using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Payload : MonoBehaviour
{

    //PathCreator path;
    [SerializeField] LayerMask canStopLayermask;
    [SerializeField] float speed = 0.05f;

    float t = 0;

    bool isInit;
    bool stop = false;

    [SerializeField] int maxPvs;
    [HideInInspector] public int PVs;

    public void Init()
    {

        isInit = true;

        PVs = maxPvs;
    }


    public void Update()
    {
        if (!isInit) return;

        bool canMove = true;

        if (Physics.OverlapBox(transform.position + transform.forward, new Vector3(transform.lossyScale.x, 1, 1f), default, canStopLayermask).Length > 0) canMove = false;
        //casts in front to check that it can move;
        if (canMove && !stop) MoveForwardOnPath();

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
        if (PVs <= 0) R.get.game.Lose();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position + transform.forward, new Vector3(transform.lossyScale.x, 1, 1f) * 2f);
    }
}
