using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MainMenu : MonoBehaviour
{
    [FoldoutGroup("Refs")] public MainMenuHorizontalScroll horizontalScroll; 
    [FoldoutGroup("Refs")] public MainMenuTabBar tabBar; 
    [FoldoutGroup("Refs")] public MainMenuPanelTop panelTop; 
    [FoldoutGroup("Refs")] public MainMenuMap map; 
    [FoldoutGroup("Refs")] public MainMenuLeaderboard leaderboard; 
    [FoldoutGroup("Refs")] public MainMenuSettings settings; 

    [HideInInspector] public float ratioWidth; //diff avec reso height de base
    

    public void Init()
    {
        float ratio = (Screen.height*1f) / (Screen.width*1f); 
        ratioWidth = 2436f / ratio;

        horizontalScroll.Init();
        tabBar.Init();
        panelTop.Init();
        map.Init();
        leaderboard.Init();
        settings.Init();

        if (ReloadSceneSettings.clickedHomeAfterWinningLevel)
            map.SetupWithTween();
        else
            map.SetupInstant();

        horizontalScroll.ScrollToScreenInstant(1);
    }
}
