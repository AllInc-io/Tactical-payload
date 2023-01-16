using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonOnOff : MonoBehaviour
{
    public Image iconON;
    public Image iconOFF;
    public TextMeshProUGUI text;
    public Image back;
    public Color colorOFF;

    public Color backColor;

    public void SetON()
    {
        iconOFF.enabled = false;
        iconON.enabled = true;


    }

    public void SetOFF()
    {
        iconOFF.enabled = true;
        iconON.enabled = false;


    }
}
