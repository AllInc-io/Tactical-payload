using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DemoSwipeController : MonoBehaviour
{
    public Character character;
    public Transform cube;
    bool onMove;

    //Awake pour les besoins de la demo, mais normalement on passe par un Init() via R
    void Awake()
    {
        character.Init();
        
    }

    public void Move(Direction direction)
    {
        onMove = true;
        
        StartCoroutine(AnimMove(direction));
    }

    IEnumerator AnimMove(Direction direction)
    {
        Vector3 newPlayerRotation = Vector3.zero;

        Vector3 moveAmount = new Vector3();

        switch(direction)
        {
            case Direction.Right: 
                newPlayerRotation.y = 90f; 
                moveAmount.x = 2f;
            break;
            case Direction.Left: 
                newPlayerRotation.y = -90f; 
                moveAmount.x = -2f;
            break;
            case Direction.Up: 
                newPlayerRotation.y = 0f; 
                moveAmount.z = 2f;
            break;
            case Direction.Down: 
                newPlayerRotation.y = 180f; 
                moveAmount.z = -2f;
            break;
        }

        character.transform.localRotation = Quaternion.Euler(newPlayerRotation);
        character.transform.DOMove(character.transform.position + moveAmount, 0.3f).SetEase(Ease.Linear);
        character.animator.SetTrigger("Run");

        cube.transform.DOMove(cube.transform.position + moveAmount, 0.3f).SetEase(Ease.OutQuad);
        
        yield return new WaitForSeconds(0.3f);

        //(R.get.game.controls as SwipeController).OnEndDummyMove();
        character.animator.SetTrigger("Idle");
        onMove = false;
    }
}
