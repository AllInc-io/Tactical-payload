using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using TMPro;

public class MainMenuButtonTab : MonoBehaviour
{
    public int ID;
    [FoldoutGroup("Refs")] public RectTransform rectTransform;
    [FoldoutGroup("Refs")] public RectTransform iconNameCont;
    [FoldoutGroup("Refs")] public TextMeshProUGUI tabName;
    [FoldoutGroup("Refs")] public Image icon;

    [HideInInspector] public Vector2 highlightSize;
    [HideInInspector] public Vector2 lowlightSize;

    Vector2 baseIconNamePos;

    public void Init()
    {
        baseIconNamePos = iconNameCont.anchoredPosition;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, R.get.mainMenu.tabBar.baseTabWidth);
        rectTransform.anchoredPosition = new Vector2(R.get.mainMenu.tabBar.baseTabWidth/2f + ID*R.get.mainMenu.tabBar.baseTabWidth, rectTransform.anchoredPosition.y);

        float highlightFactor = 1.45f;
        int nbTabs = R.get.mainMenu.tabBar.listButtonTabs.Count;

        highlightSize = new Vector2(R.get.mainMenu.tabBar.baseTabWidth*highlightFactor, rectTransform.sizeDelta.y);
        lowlightSize = new Vector2(R.get.mainMenu.tabBar.baseTabWidth*(1f-((highlightFactor-1f)/((nbTabs-1)*1f))), rectTransform.sizeDelta.y);       
    }

    public void OnClick()
    {
        R.get.mainMenu.horizontalScroll.ScrollToScreen(ID);
    }

    public void Highlight()
    {
        float destPosX;

        if(ID==0)   
            destPosX = highlightSize.x/2f;
        else    
            destPosX = lowlightSize.x/2f + (ID-1)*lowlightSize.x + lowlightSize.x/2f + highlightSize.x/2f;
        
        rectTransform.DOSizeDelta(highlightSize, 0.3f).SetEase(Ease.OutQuad);
        rectTransform.DOAnchorPos(new Vector2(destPosX, rectTransform.anchoredPosition.y), 0.3f).SetEase(Ease.OutQuad);

        iconNameCont.DOScale(1.2f, 0.5f).SetEase(Ease.OutQuad);
        iconNameCont.DOAnchorPos(new Vector2(iconNameCont.anchoredPosition.x,baseIconNamePos.y+60f), 0.5f).SetEase(Ease.OutQuad);
        tabName.enabled = true;
    }

    public void HighlightInstant()
    {
        float destPosX;

        if(ID==0)   
            destPosX = highlightSize.x/2f;
        else    
            destPosX = lowlightSize.x/2f + (ID-1)*lowlightSize.x + lowlightSize.x/2f + highlightSize.x/2f;
        
        rectTransform.sizeDelta = highlightSize;
        rectTransform.anchoredPosition = new Vector2(destPosX, rectTransform.anchoredPosition.y);
        iconNameCont.localScale = Vector3.one * 1.2f;
        iconNameCont.anchoredPosition = new Vector2(iconNameCont.anchoredPosition.x,baseIconNamePos.y+60f);
        tabName.enabled = true;
    }

    public void Lowlight()
    {
        float destPosX;
        destPosX = lowlightSize.x/2f + ID*lowlightSize.x;

        if(ID > R.get.mainMenu.horizontalScroll.currentID)
        {
            destPosX -= lowlightSize.x;
            destPosX += highlightSize.x;
        }
            
        rectTransform.DOSizeDelta(lowlightSize, 0.3f).SetEase(Ease.OutQuad);
        rectTransform.DOAnchorPos(new Vector2(destPosX, rectTransform.anchoredPosition.y), 0.3f).SetEase(Ease.OutQuad);
        
        iconNameCont.DOScale(1f, 0.3f).SetEase(Ease.OutQuad);
        iconNameCont.DOAnchorPos(new Vector2(iconNameCont.anchoredPosition.x, baseIconNamePos.y), 0.3f).SetEase(Ease.OutQuad);
        tabName.enabled = false;
    }

    public void LowlightInstant()
    {
        float destPosX;
        destPosX = lowlightSize.x/2f + ID*lowlightSize.x;

        if(ID > R.get.mainMenu.horizontalScroll.currentID)
        {
            destPosX -= lowlightSize.x;
            destPosX += highlightSize.x;
        }
            
        rectTransform.sizeDelta = lowlightSize;
        rectTransform.anchoredPosition = new Vector2(destPosX, rectTransform.anchoredPosition.y);
        iconNameCont.localScale = Vector3.one;
        iconNameCont.anchoredPosition = new Vector2(iconNameCont.anchoredPosition.x, baseIconNamePos.y);
        tabName.enabled = false;
    }


}
