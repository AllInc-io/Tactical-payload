using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SwipeController : Controller
{

    [Range(0f, 1f)]
    public float phoneScreenWidthRatioForSwipe = 0.25f;
    [Range(0f, 1f)]
    public float tabletScreenWidthRatioForSwipe = 0.125f;
    float distanceSwipe;
    float pointRefX;
    float pointRefY;
    float distanceX;
    float distanceY;
    // testing dummy, should be replaced by a ref to the GameManager
    bool onMove;

    public override void Init()
    {
        base.Init();

        // si tablet
        if((Screen.width*1f)/(Screen.height*1f) > 9f/16f)
        {
           distanceSwipe = tabletScreenWidthRatioForSwipe * Screen.width;
        }
        else // si phone
        {
            distanceSwipe = phoneScreenWidthRatioForSwipe * Screen.width;
        }
    }

    public void Update()
    {
        if(!isOn)
            return;

        if(Input.GetMouseButtonDown(0))
        {
            pointRefX = Input.mousePosition.x;
            pointRefY = Input.mousePosition.y;
        }

        if(Input.GetMouseButton(0))
        {
            distanceX = Input.mousePosition.x - pointRefX;
            distanceY = Input.mousePosition.y - pointRefY;

            if(!onMove)
            {
                 if(distanceX > distanceY)
                {
                    if(distanceX < -distanceSwipe)
                        Swipe(Direction.Left);
                    else if(distanceX > distanceSwipe)
                        Swipe(Direction.Right);
                    else if(distanceY > distanceSwipe)
                        Swipe(Direction.Up);
                    else if(distanceY < -distanceSwipe)
                        Swipe(Direction.Down);
                }
                else
                {
                    if(distanceY > distanceSwipe)
                        Swipe(Direction.Up);
                    else if(distanceY < -distanceSwipe)
                        Swipe(Direction.Down);
                    else if(distanceX < -distanceSwipe)
                        Swipe(Direction.Left);
                    else if(distanceX > distanceSwipe)
                        Swipe(Direction.Right);
                }
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
           ResetSwipe();
        }
    }

    void Swipe(Direction direction)
    {
        if(onStart) 
        {
            R.get.game.StartGame();
            onStart = false;
        }
        
        // testing dummy, should be replaced by a ref to the GameManager
        DummyMove(direction);
    }

    // DUMMY
    void DummyMove(Direction direction)
    {
        onMove = true;
        Debug.Log("SWIPE: " + direction.ToString());

        GameObject.Find("Main").GetComponent<DemoSwipeController>().Move(direction);

    }

    // DUMMY
    public void OnEndDummyMove()
    {
        ResetSwipe();
        onMove = false;
    }

    public void ResetSwipe()
    {
        pointRefX = Input.mousePosition.x;
        pointRefY = Input.mousePosition.y;
        distanceX = 0f;
        distanceY = 0f;
    }
}

public enum Direction
{
    Left,
    Up,
    Right,
    Down
}

