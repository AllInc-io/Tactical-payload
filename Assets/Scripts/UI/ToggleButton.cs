using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ToggleButton : MonoBehaviour
{
    public RectTransform toggle;
    public Image backgroundYes;
    public Image backgroundNo;
    public Image iconYes;
    public Image iconNo;
    public Image knob;
    public RectTransform baseButtonPosYes;
    [HideInInspector] public bool isOn;

    Vector3 buttonPosNo;
    Vector3 buttonPosYes;

    public void Init()
    {
        buttonPosNo = knob.transform.localPosition;
        buttonPosYes = baseButtonPosYes.transform.localPosition;

        SetOffInstant();
    }

    public void SetOn()
    {
        isOn = true;
        DOTween.Kill("TweenToggleSettings", true);

        backgroundYes.enabled = true;
        iconYes.enabled = true;
        backgroundNo.enabled = false;
        iconNo.enabled = false;
        knob.transform.localPosition = buttonPosYes;

        toggle.DOPunchScale(Vector3.one*0.1f, 0.3f, 1, 1f).SetId("TweenToggleSettings");

    }

     public void SetOnInstant()
    {
        isOn = true;
        DOTween.Kill("TweenToggleSettings", true);

        backgroundYes.enabled = true;
        iconYes.enabled = true;
        backgroundNo.enabled = false;
        iconNo.enabled = false;
        knob.transform.localPosition = buttonPosYes;
    }

    public void SetOff()
    {
        isOn = false;
        DOTween.Kill("TweenToggleSettings", true);
        
        backgroundYes.enabled = false;
        iconYes.enabled = false;
        backgroundNo.enabled = true;
        iconNo.enabled = true;
        knob.transform.localPosition = buttonPosNo;

        toggle.DOPunchScale(Vector3.one*0.1f, 0.3f, 1, 1f).SetId("TweenToggleSettings");
    }

     public void SetOffInstant()
    {
        isOn = false;
        DOTween.Kill("TweenToggleSettings", true);
        
        backgroundYes.enabled = false;
        iconYes.enabled = false;
        backgroundNo.enabled = true;
        iconNo.enabled = true;
        knob.transform.localPosition = buttonPosNo;
    }

    
}
