using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class RessourcePanel : MonoBehaviour
{
    [FoldoutGroup("Refs")] public TextMeshProUGUI text;
    [FoldoutGroup("Refs")] public RectTransform animAnchor;


    public void Init()
    {

    }

    public void Show()
    {
        gameObject.SetActive(true);

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateValue(int newValue)
    {
        text.text = "" + newValue;
    }
}
