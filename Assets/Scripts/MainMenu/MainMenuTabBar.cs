using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class MainMenuTabBar : MonoBehaviour
{
    [FoldoutGroup("Refs")] public RectTransform highlighter;
    [FoldoutGroup("Refs")] public List<MainMenuButtonTab> listButtonTabs;
    [HideInInspector] public float baseTabWidth;


    public void Init()
    {
        baseTabWidth = R.get.mainMenu.ratioWidth/listButtonTabs.Count;

        for(int i=0; i < listButtonTabs.Count; i++)
        {
            listButtonTabs[i].Init();
        }
    }

    public void UpdateStatus()
    {
        int currentScreen = R.get.mainMenu.horizontalScroll.currentID;

        for(int i=0; i < listButtonTabs.Count; i++)
        {
            if(i==currentScreen)
            {
                listButtonTabs[i].Highlight();
            }
            else
            {
                listButtonTabs[i].Lowlight();
            }
        }

        float destPosX;

        if(currentScreen==0)   
            destPosX = listButtonTabs[0].highlightSize.x/2f;
        else    
            destPosX = listButtonTabs[0].lowlightSize.x/2f + (currentScreen-1)*listButtonTabs[0].lowlightSize.x + listButtonTabs[0].lowlightSize.x/2f + listButtonTabs[0].highlightSize.x/2f;
        
        highlighter.DOSizeDelta(listButtonTabs[0].highlightSize, 0.3f).SetEase(Ease.OutQuad);
        highlighter.DOAnchorPos(new Vector2(destPosX, highlighter.anchoredPosition.y), 0.4f).SetEase(Ease.OutQuad);
    }

    public void UpdateStatusInstant()
    {
        int currentScreen = R.get.mainMenu.horizontalScroll.currentID;

        for(int i=0; i < listButtonTabs.Count; i++)
        {
            if(i==currentScreen)
            {
                listButtonTabs[i].HighlightInstant();
            }
            else
            {
                listButtonTabs[i].LowlightInstant();
            }
        }

        float destPosX;

        if(currentScreen==0)   
            destPosX = listButtonTabs[0].highlightSize.x/2f;
        else    
            destPosX = listButtonTabs[0].lowlightSize.x/2f + (currentScreen-1)*listButtonTabs[0].lowlightSize.x + listButtonTabs[0].lowlightSize.x/2f + listButtonTabs[0].highlightSize.x/2f;
        
        highlighter.sizeDelta = listButtonTabs[0].highlightSize; 
        highlighter.anchoredPosition = new Vector2(destPosX, highlighter.anchoredPosition.y);    
    }
}
