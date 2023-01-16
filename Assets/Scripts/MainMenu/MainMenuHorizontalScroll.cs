using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MainMenuHorizontalScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [FoldoutGroup("Refs")] public RectTransform scrollableContent;
    [FoldoutGroup("Refs")] public ScrollRect scrollView;
    [FoldoutGroup("Refs")] public Canvas baseCanvas;
    [FoldoutGroup("Refs")] public List<RectTransform> listPanels;
    
    [HideInInspector] public int currentID;

    public void Init()
    {
        scrollableContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, R.get.mainMenu.ratioWidth*listPanels.Count);

        for(int i=0; i< listPanels.Count; i++)
        {
            listPanels[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, R.get.mainMenu.ratioWidth);
            listPanels[i].anchoredPosition = new Vector2(R.get.mainMenu.ratioWidth*i, listPanels[i].anchoredPosition.y);
        }
    }

    public void ScrollToScreen(int idDest)
    {
        if(idDest > 4 ||idDest < 0)
            return;

        currentID = idDest;

        float pos = idDest*R.get.mainMenu.ratioWidth/(R.get.mainMenu.ratioWidth*(listPanels.Count-1));
        scrollView.DOHorizontalNormalizedPos(pos, 0.25f, false).SetEase(Ease.OutCubic);

        R.get.mainMenu.tabBar.UpdateStatus();
    }

    public void ScrollToScreenInstant(int idDest)
    {
        if(idDest > 4 || idDest < 0)
            return;

        currentID = idDest;

        float pos = idDest*R.get.mainMenu.ratioWidth/(R.get.mainMenu.ratioWidth*(listPanels.Count-1));
        scrollView.horizontalNormalizedPosition = pos;

        R.get.mainMenu.tabBar.UpdateStatusInstant();
    }

    public void OnBeginDrag(PointerEventData data)
    {
        
    }

    public void OnDrag(PointerEventData data)
    {
        
    }

   public void OnEndDrag(PointerEventData data)
    {
        float difference = data.pressPosition.x - data.position.x;

        if(Mathf.Abs(difference) >= R.get.mainMenu.ratioWidth / 10f)
        {
            if(difference>0)
                ScrollToScreen(currentID+1);
            else
                ScrollToScreen(currentID-1);
        }   
        else
        {
           ScrollToScreen(currentID);
        }
    }
}
