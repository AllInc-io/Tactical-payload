using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoHorizontalController : MonoBehaviour
{
    public Character character;
    public float cameraToCharacterMovementRatio = 0.4f;
    public float cameraSpeedLerp = 0.1f;
    public float characterSpeedLerp = 0.25f;
    public float verticalSpeed = 6f;
    float newPosXChar;

    bool onMove;
    bool gameOver;

    //Awake pour les besoins de la demo, mais normalement on passe par un Init() via R
    void Awake()
    {
        character.Init();
    }

    public void Move(float newPosition)
    {
        if(gameOver)
            return;

        //newPosXChar = Mathf.Max(MoveLimitLeft, Mathf.Min(MoveLimitRight, newPosition));

        Vector3 newCharacterPos = new Vector3(newPosXChar, character.transform.localPosition.y, character.transform.localPosition.z);
        character.transform.localPosition = Vector3.Lerp(character.transform.localPosition, newCharacterPos, characterSpeedLerp);  
    }

    void Update()
    {
        if(R.get.game.controls.IsOn)
        {
            character.animator.SetTrigger("Run");
            transform.position += transform.forward*Time.deltaTime*verticalSpeed;
        }
            
        Vector3 cameraDest = R.get.mainCamera.transform.localPosition;
        cameraDest.x = (character.transform.localPosition.x) * cameraToCharacterMovementRatio;
        R.get.mainCamera.transform.localPosition = Vector3.Lerp(R.get.mainCamera.transform.localPosition, cameraDest, cameraSpeedLerp);
    }


    public void CollideWithCube()
    {
        character.ActivateRagdoll(-Vector3.forward);
        gameOver = true;

        R.get.game.Lose();
    }

    public void CollideWithCoin()
    {
        
    }

    public void EnterWinTrigger()
    {
        R.get.game.Win();

        character.SetIdle();
    }


}
