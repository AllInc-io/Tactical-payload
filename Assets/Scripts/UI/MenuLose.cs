using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using TMPro;

public class MenuLose : MonoBehaviour
{

    public void Init()
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
        //R.get.ui.menuBank.AnimateRessourcesGoingIntoBank(R.get.levelManager.level.currentZone.transform.position, 30, Vector2.one * 10);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ClickButtonRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
