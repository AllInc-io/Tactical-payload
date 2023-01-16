using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class R : MonoBehaviour
{
    public static R get;
    
    public Mode mode;
    public FakeLeaderboard leaderboard;
    public LevelDesignScriptableObject levelDesign;

    [HideInInspector] public GameManager game;
    [HideInInspector] public LevelManager levelManager;
    [HideInInspector] public UIManager ui;
    [HideInInspector] public HapticManager haptics;
    [HideInInspector] public MainMenu mainMenu;
    [HideInInspector] public int lastLevelFinished;
    [HideInInspector] public Camera mainCamera;
    [HideInInspector] public int score;

    [HideInInspector] public bool hasMusic;
    [HideInInspector] public bool hasSFX;

    [HideInInspector] public GameObject collidingWith;

    [HideInInspector] public MenuStart startMenu;



    void Awake()
    {
        if(get==null)
        {
            get = this;
        }

        Application.targetFrameRate = 60;

        if(PlayerPrefs.GetInt("HasInit") == 0)
        {
            PlayerPrefs.SetInt("HasInit", 1);
            PlayerPrefs.SetInt("HasHaptic", 1);
            PlayerPrefs.SetInt("HasMusic", 1);
            PlayerPrefs.SetInt("HasSFX", 1);
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.SetInt("LastLevelFinished", 0);

            string team = "";
            for(int i = 0; i < 3; i++)
            {
                team += levelDesign.possibleCharactersPrefabs[i].heroName;
                if(i < 2) team += ",";
            }
            PlayerPrefs.SetString("Team", team);

            foreach(Hero hero in levelDesign.possibleCharactersPrefabs)
            {
                PlayerPrefs.SetInt("Level" + hero.heroName, 0);
            }
        }

        lastLevelFinished = PlayerPrefs.GetInt("LastLevelFinished", 0);
        score = PlayerPrefs.GetInt("Score", 0);
        hasMusic = PlayerPrefs.GetInt("HasMusic", 0)==0?false:true;
        hasSFX = PlayerPrefs.GetInt("HasSFX", 0)==0?false:true;

        mainCamera = Camera.main;

        if (mode == Mode.Game)
        {
            game = transform.GetComponent<GameManager>();
            levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            ui = GameObject.Find("UIManager").GetComponent<UIManager>();
            haptics = transform.GetComponent<HapticManager>();

            haptics.Init();
            levelManager.Init();
            game.Init();
            ui.Init();

            game.StartGame();
        }
        else if (mode == Mode.Menu)
        {
            //mainMenu = GameObject.Find("MainMenu").GetComponent<MainMenu>();

            game = transform.GetComponent<GameManager>();
            haptics = transform.GetComponent<HapticManager>();
            startMenu = FindObjectOfType<MenuStart>();
            FindObjectOfType<MenuBank>().Init();
            haptics.Init();
            game.MenuInit();
            startMenu.Init();

            //mainMenu.Init();
        }

        ReloadSceneSettings.Clear();
    }

    public void LoadLevel(int levelID)
    {
        //dummy pour la demo
        SceneManager.LoadScene("DrawController");
    }

    public void AddScore(int scoreToAdd=1)
    {
        score += scoreToAdd;

        if (mode == Mode.Game)
        {
            ui.menuBank.UpdateGold();

        }

        else if (mode == Mode.Menu)
        {
            FindObjectOfType<MenuBank>().ressourcePanel.UpdateValue(score);
            startMenu.characterSelectionMenu.CheckAllScoreUpdateButton();
        }

        PlayerPrefs.SetInt("Score", score);
    }





    [Button(ButtonSizes.Large)]
    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}

public static class ReloadSceneSettings 
{
    public static bool clickedHomeAfterWinningLevel = false;

    public static void Clear()
    {
        clickedHomeAfterWinningLevel = false;
    }
}


public enum Mode
{
    Game,
    Menu
}
