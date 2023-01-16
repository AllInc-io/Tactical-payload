using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MainMenuMap : MonoBehaviour
{
    [FoldoutGroup("Refs")] public List<ItemMap> listItems;
    [FoldoutGroup("Refs")] public ScrollRectEx scrollView;
    [FoldoutGroup("Refs")] public RectTransform scrollableContent;
    [FoldoutGroup("Refs")] public float offsetBottom;

    public void Init()
    {
   
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetupInstant()
    {
        int currentLevelID = 0;
        int playerLevel = R.get.lastLevelFinished+1;

        int k = 2;

        if(playerLevel <= 2)
            k = playerLevel-1;

        for(int i=0; i < listItems.Count; i++)
        {
            int currentLevel = playerLevel-(k-i);
            listItems[i].Init();

            if(currentLevel < playerLevel)
                listItems[i].Setup(currentLevel, LevelMapState.Played, false, false);
            else if(currentLevel == playerLevel)
            {
                currentLevelID = i;
                listItems[i].Setup(currentLevel, LevelMapState.Current, false, false);
            }
            else if(currentLevel >= 1)
                listItems[i].Setup(currentLevel, LevelMapState.Locked, false, false);
        }
        
        float offsetTop = listItems[listItems.Count-1].rectTransform.anchoredPosition.y;

        if (currentLevelID == 0) offsetBottom = 500;
        else if (currentLevelID == 1) offsetBottom = 250f;

        float contentHeight = Mathf.Abs(offsetTop) + (listItems.Count-1)*500f + offsetBottom;

        scrollableContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);

        float pos = 0.18f * (currentLevelID -2);
        scrollView.verticalNormalizedPosition = pos;

        /*
        if(currentLevelID == 0)
            scrollView.verticalNormalizedPosition = 0f;
        else  if(currentLevelID == 1)
            scrollView.verticalNormalizedPosition = 0.131f;
        else  if(currentLevelID == 2)
            scrollView.verticalNormalizedPosition = 0.261f;*/
    }

    public void SetupWithTween()
    {
        int currentLevelID = 0;
        int playerLevel = R.get.lastLevelFinished+1;

        int k = 2;

        if(playerLevel <= 2)
            k = playerLevel-1;

        for(int i=0; i < listItems.Count; i++)
        {
            int currentLevel = playerLevel-(k-i);
            listItems[i].Init();

            if(currentLevel < playerLevel)
            {
                listItems[i].Setup(currentLevel, LevelMapState.Played, false, false);

                if(currentLevel == playerLevel-1)
                {
                    currentLevelID = i;
                    listItems[i].ForceEmptyPath();
                }
                    
            }
            else if(currentLevel >= 1)
                listItems[i].Setup(currentLevel, LevelMapState.Locked, false, false);
        }

        if (currentLevelID == 0) offsetBottom = 500;
        else if (currentLevelID == 1) offsetBottom = 250f;


        float offsetTop = listItems[listItems.Count-1].rectTransform.anchoredPosition.y;
        float contentHeight = Mathf.Abs(offsetTop) + (listItems.Count-1)*500f + offsetBottom;

        scrollableContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);

        float pos = 0.18f * (currentLevelID - 2);
        scrollView.verticalNormalizedPosition = pos;

        /*
        if (currentLevelID == 0)
            scrollView.verticalNormalizedPosition = 0f;
        else if(currentLevelID == 1)
            scrollView.verticalNormalizedPosition = 0.131f;
        else  
            scrollView.verticalNormalizedPosition = 0.261f;*/

        StartCoroutine(AnimUnlockNewLevel(currentLevelID));
    }

    IEnumerator AnimUnlockNewLevel(int currentLevelID)
    {
        yield return new WaitForSeconds(0.4f);

        float dest = 0f;

        
        if (currentLevelID+1 == 1)
            dest = 0.131f;
        else  
            dest = 0.18f;

        scrollView.DOVerticalNormalizedPos(dest, 0.9f).SetEase(Ease.OutQuad);

        listItems[currentLevelID].TweenFillPath();

        yield return new WaitForSeconds(0.9f);

        listItems[currentLevelID+1].FlashToCurrent();
    }

    public void ClickPlayButton()
    {
        R.get.LoadLevel(R.get.lastLevelFinished+1);
    }

    public void Update()
    {
        //Debug.Log("vertical scroll = " + scrollView.verticalNormalizedPosition);
    }



    



}
