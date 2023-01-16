using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DemoVirtualJoystickController : MonoBehaviour
{

    [FoldoutGroup("Refs")] public Character character;
    [FoldoutGroup("Refs")] public Rigidbody testcube;
    public float speed = 0.1f;


    //Awake pour les besoins de la demo, mais normalement on passe par un Init() via R
    void Awake()
    {
        character.Init();
    }

    public void Move(Vector3 direction)
    {
        character.rig.MovePosition(character.transform.position+direction*speed);
        character.transform.LookAt(character.transform.position+direction*speed, transform.up);
        character.animator.ResetTrigger("Idle");
        character.animator.SetTrigger("Run");
        //testcube.MovePosition(testcube.transform.position+direction*speed);
    }

    public void StopMove()
    {
        character.animator.ResetTrigger("Run");
        character.animator.SetTrigger("Idle");
    }
}
