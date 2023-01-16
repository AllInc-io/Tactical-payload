using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalController : Controller
{
    [Range(0.1f, 1.5f)]
    public float phoneHorizontalSpeed = 0.6f;
    [Range(0.1f, 1.5f)]
    public float tabletHorizontalSpeed = 1f;
    public float moveLimitLeft = -3f;
    public float moveLimitRight = 3f;
    Vector3 moveableBasePos;
    float widthController;
    float widthLevel;
    float pointRefX;
    float distanceX;

    public override void Init()
    {
        base.Init();

        // if tablet
        if((Screen.width*1f)/(Screen.height*1f) > 9f/16f)
        {
            widthController = Screen.height * (9f/16f) * (1.6f-tabletHorizontalSpeed);
        }
        else // if phone
        {
            widthController = Screen.width * (1.6f-phoneHorizontalSpeed);
        }

        widthLevel = moveLimitRight - moveLimitLeft;

        // requires ButtonStart on UI to begin
        isOn = false;
    }

    void Update()
    {
        if(!isOn)
            return;

        if(Input.GetMouseButtonDown(0))
        {
            pointRefX = Input.mousePosition.x;
            moveableBasePos = GameObject.Find("Main").GetComponent<DemoHorizontalController>().character.transform.localPosition;
        }

        if(Input.GetMouseButton(0))
        {
            float distanceX = Input.mousePosition.x - pointRefX;

            // testing dummy, should be replaced by a ref to the GameManager
            DummyMove(moveableBasePos.x + (distanceX / widthController) * widthLevel);
        }
    }

     
    // DUMMY
    void DummyMove(float newPosition)
    {
        GameObject.Find("Main").GetComponent<DemoHorizontalController>().Move(newPosition);
    }
}
