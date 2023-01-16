using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UIManager : MonoBehaviour
{

    [FoldoutGroup("Refs")] public ButtonOnOff buttonHaptic;
    [FoldoutGroup("Refs")] public MenuStart menuStart;
    [FoldoutGroup("Refs")] public MenuIngame menuIngame;
    [FoldoutGroup("Refs")] public MenuBank menuBank;
    [FoldoutGroup("Refs")] public MenuWin menuWin;
    [FoldoutGroup("Refs")] public MenuLose menuLose;

    
    public TextMeshProUGUI levelText;
    public Transform iconHapticON;
    public Transform iconHapticOFF;
    bool onStart;
    bool isInit;
    Vector3 basePosCombo;
    Coroutine comboNumberCoroutine;


    public void Init()
    {
        onStart = true;

        //menuStart.Init();
        menuIngame.Init();
        menuBank.Init();
        menuWin.Init();
        menuLose.Init();

        //menuStart.Show();
        menuBank.Show();
        menuIngame.Hide();
        menuWin.Hide();
        menuLose.Hide();

        isInit = true;
    }



   

    


}
