using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class VirtualJoystickController : Controller
{

    [FoldoutGroup("Refs")] public VirtualJoystick joystick;
    [HideInInspector] public Vector3 moveDirection;
    bool joystickOn;

    Vector2 joystickOrigin;
   

    public override void Init()
    {
        base.Init();

        joystick.Init();
        HideJoystick();

        isOn = false;
    }

    void Update()
    {
        if(!isOn)
            return;

        if(Input.GetMouseButtonDown(0))
        {
            //place the joystick
            joystickOn = true;
            joystickOrigin = Input.mousePosition;
            Vector3 mousePos = joystickOrigin;
            mousePos.z = 0.5f;
            ShowJoystickAt(R.get.mainCamera.ScreenToWorldPoint(mousePos));
        }

        if(joystickOn)
        {
            if(Input.GetMouseButton(0))
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 0.5f;

                float circleRadius = 0.5f;
                joystick.cursor.transform.position = R.get.mainCamera.ScreenToWorldPoint(mousePos);
                Vector3 cursorLocalPos = joystick.cursor.transform.localPosition;
                Vector3 cursorPerimeterPos;

                float angleCursor = Vector2.SignedAngle(Vector3.zero + joystick.cursor.transform.localPosition, joystick.cursor.transform.up);

                if(angleCursor < 0)
                    angleCursor += 360f;

                if(Vector3.Distance(cursorLocalPos, Vector3.zero) > circleRadius)
                {
                    float posX = 0f + (Mathf.Sin(angleCursor * Mathf.Deg2Rad) * circleRadius);
                    float posY = 0f + (Mathf.Cos(angleCursor * Mathf.Deg2Rad) * circleRadius);

                    cursorPerimeterPos = new Vector3(posX, posY, 0f); 
                }
                else
                {
                    cursorPerimeterPos = cursorLocalPos;
                }

                joystick.cursor.transform.localPosition = cursorPerimeterPos;

                
                // testing dummy, should be replaced by a ref to the GameManager

                moveDirection = cursorPerimeterPos.normalized;
                moveDirection.z = moveDirection.y;
                moveDirection.y = 0f;
                
            }
            else
            {
                // remove joystick
                moveDirection = Vector3.zero;
                joystickOn = false;
                HideJoystick();
            }
        }

        
    }

    void FixedUpdate()
    {
        if(moveDirection == Vector3.zero)
            DummyStopMove();
        else
            DummyMove(moveDirection);
    }

    public void UpdateCursor(Vector3 position)
    {
        
    }

    public void ShowJoystickAt(Vector3 position)
    {
        joystick.gameObject.SetActive(true);
        joystick.transform.position = position;
        joystick.cursor.transform.localPosition = Vector3.zero;
    }

    public void HideJoystick()
    {
        joystick.gameObject.SetActive(false);
    }

     
    // DUMMY
    void DummyMove(Vector3 directionMove)
    {
        GameObject.Find("Main").GetComponent<DemoVirtualJoystickController>().Move(directionMove);
    }

    void DummyStopMove()
    {
        GameObject.Find("Main").GetComponent<DemoVirtualJoystickController>().StopMove();
    }
}
