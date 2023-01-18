using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
public class GameManager : SerializedMonoBehaviour
{
    [FoldoutGroup("Refs")] public DrawController controls;
    [FoldoutGroup("Refs")] public GameObject winFX;
    [FoldoutGroup("Refs"), SerializeField] Transform cameraLookAt;



    [HideInInspector] public Vector3 startCameraPos;
    [HideInInspector] public Hero[] heroes;

    public bool isOn = false;

    bool won = false;

    public int currentLevel = 1;
    public int currentXP { get; private set; }

    [SerializeField] int startXPToNextLevel; // will double every level

    int xpToNextLevel;

    public void Init()
    {
        controls.Init();

        //R.get.levelManager.level.currentZone.SpawnCharas(heroes);

        heroes = new Hero[3];

        string[] heroesNames = PlayerPrefs.GetString("Team").Split(',');
        if (heroesNames.Length <= 3)
        {
            heroesNames = new string[3];
            for(int i = 0; i< 3; i++)
            {
                heroesNames[i] = R.get.levelDesign.possibleCharactersPrefabs[i].heroName;
            }

        }

        for(int i = 0; i < 3; i++)
        {
            Vector3 pos = Vector3.forward * i * -3f;
            Vector3 scale = Vector3.one * R.get.levelDesign.charactersScale;

            heroes[i] = Instantiate(R.get.levelDesign.possibleCharactersPrefabs.Where(hero => hero.name.Equals(heroesNames[i])).FirstOrDefault());
            heroes[i].transform.position = pos;
            heroes[i].transform.localScale = scale;


        }
        if (R.get.lastLevelFinished < R.get.levelDesign.levelUnlockSecondChara - 1)
        {
            heroes[1].dead = true;
            heroes[1].gameObject.SetActive(false);
        }
        if (R.get.lastLevelFinished < R.get.levelDesign.levelUnlockThirdChara - 1)
        {
            heroes[2].dead = true;
            heroes[2].gameObject.SetActive(false);
        }

        xpToNextLevel = startXPToNextLevel;

    }

    public void MenuInit()
    {
        heroes = new Hero[3];
    }

    public void Update()
    {
        //if(isOn) MoveCamera();

    }



    public void StartGame()
    {
        startCameraPos = R.get.mainCamera.transform.position;
        startCameraPos.y = 0;

        R.get.ui.menuStart.Hide();
        R.get.ui.menuIngame.Show();
        controls.OnStart();

        foreach (Character character in heroes)
        {
            character.Init();
        }

        R.get.levelManager.level.StartLevel();

        isOn = true;
    }

    public void Win()
    {
        if (won) return;
        won = true;
        R.get.lastLevelFinished++;
        PlayerPrefs.SetInt("LastLevelFinished", R.get.lastLevelFinished);

        winFX.SetActive(true);
        controls.OnWin();

        R.get.ui.menuIngame.Hide();
        R.get.ui.menuWin.Show();
    }

    public void Lose()
    {
        controls.Stop();

        R.get.ui.menuIngame.Hide();
        R.get.ui.menuLose.Show();
    }

    public void GetXP(int amount)
    {
        currentXP += amount;
        if(currentXP >= )
    }
}
