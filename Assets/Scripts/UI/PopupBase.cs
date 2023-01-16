using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

public class PopupBase : MonoBehaviour
{

    [FoldoutGroup("Refs"), SerializeField] public RectTransform halo;
    [FoldoutGroup("Refs"), SerializeField] public Image bg;
    [FoldoutGroup("Refs"), SerializeField] public Image cartel;
    [FoldoutGroup("Refs"), SerializeField] public RectTransform topIcon;

    bool isClosing = false;

    Color baseBgColor;


    public virtual void Init()
    {
        baseBgColor = bg.color;
        HideInstant();
    }

    public virtual void Show()
    {
        isClosing = false;

        gameObject.SetActive(true);

        cartel.transform.localScale = Vector3.zero;
        cartel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack) ;

        bg.color = new Color(0, 0, 0, 0);
        bg.DOColor(baseBgColor, 0.5f);

        if(topIcon != null) topIcon.DOLocalMoveY(topIcon.transform.localPosition.y + 80f, 0.6f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        halo.DOLocalRotate(new Vector3(0f, 0f, 360f), 15f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }

    public virtual void Hide()
    {
        if (isClosing) return;
        bg.DOColor(new Color(0, 0, 0, 0), 0.5f);
        cartel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
        isClosing = true;
    }

    public virtual void HideInstant()
    {
        gameObject.SetActive(false);
    }



}
