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

    [FoldoutGroup("Settings"), SerializeField] float skyboxPanSpeed = 0.2f; 
    public void Init()
    {
        
        characterSelectionMenu.Init();

        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        levelText.text = "TOTAL TRAVELS:\n" + (R.get.score) + " yards";

        if(HapticManager.instance.HapticActived)
            buttonHaptic.SetON();
        else
            buttonHaptic.SetOFF();
    }

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyboxPanSpeed);
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
