using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuStart : MonoBehaviour
{
    [FoldoutGroup("Refs")] public TextMeshProUGUI levelText;
    [FoldoutGroup("Refs")] public ButtonOnOff buttonHaptic;
    [FoldoutGroup("Refs")] public CharacterSelectionMenu characterSelectionMenu;
    [FoldoutGroup("Refs")] public Transform startButton;

    public void Init()
    {
        
        characterSelectionMenu.Init();

        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        levelText.text = "LEVEL " + (R.get.lastLevelFinished+1);

        if(HapticManager.instance.HapticActived)
            buttonHaptic.SetON();
        else
            buttonHaptic.SetOFF();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ClickButtonHaptic()
    {
        HapticManager.instance.ChangeHaptic();


        if(HapticManager.instance.HapticActived)
            buttonHaptic.SetON();
        else
            buttonHaptic.SetOFF();
    }

    public void ClickButtonStart()
    {
        SceneManager.LoadScene(2);
    }

    public void SetButtonStartPlayable()
    {
        startButton.GetComponentInChildren<Button>().enabled = true;
        startButton.Find("bg").gameObject.SetActive(true);
    }

}
