using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class MenuIngame : MonoBehaviour
{


    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] RectTransform pointer;

    [SerializeField] TextMeshProUGUI waveIncomingText;
    [SerializeField] TextMeshProUGUI progressionText;


    [SerializeField] public ZoneFinishedPopup zoneFinishedPopup;

    public DeleteDrawingButton deleteDrawingButton;

    public GameObject dotProgressionPrefab;
    public Image fillProgression;


    private List<GameObject> dots;

    [SerializeField] Image xpBar;
    public TextMeshProUGUI xpBarText;
    [SerializeField] RectTransform ameliorationMenu;
    [SerializeField] RectTransform resurectionButton;

    [SerializeField] RectTransform towardsPayloadPointer;
    [SerializeField] Transform buttonsParent;

    public void Init()
    {
        waveIncomingText.transform.localScale = Vector3.zero;
        //waveIncomingArrowsParent.localScale = Vector3.zero;


        InitProgression();
        zoneFinishedPopup.Init();

        HideTutorialText();
        HidePointer();



        ameliorationMenu.gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void IndicateWaveIncoming(Vector3 direction, int index, int totalAmount)
    {

        direction.Normalize();

        DOTween.Kill(this, "WaveIncomingUI");
        Sequence sequence = DOTween.Sequence().SetId("WaveIncomingUI");

        waveIncomingText.text = "Wave incoming";

        //waveIncomingArrow.up = new Vector3(direction.x, direction.z, 0);
        //waveIncomingArrow.anchoredPosition = new Vector2(Mathf.RoundToInt(direction.x) * Screen.width * 0.4f, Mathf.RoundToInt(direction.z) * Screen.height * 0.4f);

        sequence.Append(waveIncomingText.transform.DOScale(Vector3.one, 0.5f));
       // sequence.Join(arrow.transform.DOScale(Vector3.one, 0.5f));
        
        sequence.Append(waveIncomingText.transform.DOScale(Vector3.zero, 0.5f).SetDelay(1.5f));
        //sequence.Join(arrow.transform.DOScale(Vector3.zero, 0.5f));
    }

    public void InitProgression()
    {
        /*
        dots = new List<GameObject>();

        Transform parent = transform.Find("LevelProgression").Find("Points");

        for (int i = 0; i < R.get.levelManager.level.zonesCount; i++)
        {
            GameObject dot = Instantiate(dotProgressionPrefab, parent);
            dots.Add(dot);
            dot.transform.Find("unlocked").gameObject.SetActive(false);
        }

        GridLayoutGroup gridLayoutGroup = parent.GetComponent<GridLayoutGroup>();

        int pointsAmount = R.get.levelManager.level.zonesCount;
        if (pointsAmount > 2)
        {
            gridLayoutGroup.spacing = new Vector2((gridLayoutGroup.spacing.x + gridLayoutGroup.cellSize.x) * 2f / (float)pointsAmount - gridLayoutGroup.cellSize.x, gridLayoutGroup.spacing.y);
        }

        fillProgression.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(gridLayoutGroup.cellSize.x * dots.Count + gridLayoutGroup.spacing.x * (dots.Count - 1) - 20f, fillProgression.transform.parent.GetComponent<RectTransform>().sizeDelta.y);
        fillProgression.fillAmount = 0;
        */
    }

    public void SetPointer(Vector3 worldPos, Vector3 goToWorldPos)
    {
        CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();

        pointer.DOKill();
        pointer.gameObject.SetActive(true);


        pointer.anchoredPosition = U.WorldToUIPos(worldPos, R.get.mainCamera, canvasScaler);
        pointer.DOAnchorPos(U.WorldToUIPos(goToWorldPos, R.get.mainCamera, canvasScaler), 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Restart).SetUpdate(true);
        
    }

    public void HidePointer()
    {
        pointer.DOKill();
        pointer.gameObject.SetActive(false);
    }

    public void ShowTutorialText(string text)
    {
        tutorialText.text = text;
        tutorialText.transform.parent.gameObject.SetActive(true);
    }

    public void HideTutorialText()
    {
        tutorialText.transform.parent.gameObject.SetActive(false);
    }


    public void UpdateProgression(int step = 0)
    {

        if (step > 0)
        {
            if (dots.Count > 1) fillProgression.DOFillAmount((float)(step - 1) / (float)(dots.Count - 1), 0.5f);
            //if (dots.Count > 1) fillProgression.fillAmount = (float)(step - 1) / (float)(dots.Count - 1);
            GameObject dot = dots[step - 1];
            dot.transform.Find("unlocked").gameObject.SetActive(true);
            dot.transform.Find("unlocked").localScale = Vector3.zero;
            dot.transform.Find("unlocked").DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(0.5f);
        }

    }


    public void IndicatePayload(Vector3 pos)
    {
        if (R.get.game.CheckIfPositionIsInView(pos)) towardsPayloadPointer.gameObject.SetActive(false);
        else
        {
            CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
            Vector2 res = canvasScaler.referenceResolution;
            //Vector2 ratio = new Vector2(res.x / canvasScaler.matchWidthOrHeight * Screen.width, res.y / Screen.height);

            //because it's in matchHeight fully !!
            float ratioFloat = res.y / Screen.height;
            Vector2 resPos = new Vector2(Screen.width, Screen.height);
            resPos *= ratioFloat;


            towardsPayloadPointer.gameObject.SetActive(true);
            Vector2 newPos = U.WorldToUIPos(pos, R.get.mainCamera, canvasScaler);
            newPos.x = Mathf.Clamp(newPos.x, 0, resPos.x);
            newPos.y = Mathf.Clamp(newPos.y, 0, resPos.y);
            towardsPayloadPointer.anchoredPosition = newPos;

            Vector2 angleEuler = new Vector2(Screen.width / 2f, Screen.height / 2f) - towardsPayloadPointer.anchoredPosition * new Vector2(-1, 1);


            float angle = Vector2.SignedAngle(Vector2.up, angleEuler);

            if (angleEuler.x > 0) angle = 180 - angle;

            towardsPayloadPointer.rotation = Quaternion.Euler(0, 0, angle);
        }
    }


    public RectTransform SpawnIndicator(RectTransform prefab)
    {
        RectTransform indicator = Instantiate(prefab, transform.Find("EnemyIndicators"));

        indicator.transform.localScale = Vector3.zero;
        indicator.DOScale(Vector3.one, 0.5f);

        return indicator;
    }


    public Vector2 GetIndicatorPos(Vector3 worldPos, Vector2 sizeDelta)
    {
        CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
        Vector2 res = canvasScaler.referenceResolution;
        //Vector2 ratio = new Vector2(res.x / canvasScaler.matchWidthOrHeight * Screen.width, res.y / Screen.height);

        //because it's in matchHeight fully !!
        float ratioFloat = res.y / Screen.height;
        Vector2 resPos = new Vector2(Screen.width, Screen.height);
        resPos *= ratioFloat;

        Vector2 newPos = U.WorldToUIPos(worldPos, R.get.mainCamera, GetComponentInParent<CanvasScaler>());
        newPos.x = Mathf.Clamp(newPos.x, 0, resPos.x - sizeDelta.x);
        newPos.y = Mathf.Clamp(newPos.y, 0, resPos.y - sizeDelta.y);
        return newPos;

    }

    public void IndicateProgression(int meters, bool passingLandmark)
    {
        progressionText.text = meters + " yards";

        //if (passingLandmark) progressionText.transform.DOPunchScale(Vector3.one * 1.5f, 0.5f);
    }


    public void ShowAmeliorationMenu()
    {

        bool needHeal = false;

        foreach(Hero hero in R.get.game.heroes)
        {
            if (!hero.IsAtMaxLife()) needHeal = true;
        }

        ameliorationMenu.gameObject.SetActive(true);

        if (R.get.game.heroes[0].dead || R.get.game.heroes[1].dead) resurectionButton.gameObject.SetActive(true);
        else resurectionButton.gameObject.SetActive(false);

        List<int> buttonsActivated = new List<int>();
        int newButton;
        for (int i = 0; i < 3; i++)
        {
            do
            {
                newButton = Random.Range(0, buttonsParent.childCount);

            } while (buttonsActivated.Contains(newButton) || (!needHeal && buttonsParent.GetChild(i).CompareTag("HealButton")));
            buttonsActivated.Add(newButton);


        }
        foreach(Transform child in buttonsParent)
        {
            if (!buttonsActivated.Contains(child.GetSiblingIndex())) child.gameObject.SetActive(false);
            else child.gameObject.SetActive(true);
        }
    }


    public void HideAmeliorationMenu()
    {
        ameliorationMenu.gameObject.SetActive(false);
    }

    public void SetXPBar(int currentAmount, int goal)
    {

        float fill = (float) currentAmount / goal;

        xpBar.DOKill();
        if (fill > xpBar.fillAmount) xpBar.DOFillAmount(fill, 0.3f);
        else xpBar.fillAmount = fill;

        xpBarText.text = currentAmount + "/" + goal + "xp";
    }


    
    public void LevelUpCharacters()
    {
        foreach(Hero hero in R.get.game.heroes)
        {
            hero.LevelUp();
        }


        OnBoostChosen();

    }

    public void FasterPayload()
    {
        R.get.levelManager.level.payload.SetFaster(1.5f);

        OnBoostChosen();
    }

    public void MorePayloadPVs()
    {
        R.get.levelManager.level.payload.SetMorePVs(1.5f);

        OnBoostChosen();
    }


    public void HealTeam()
    {
        R.get.levelManager.level.payload.SetMorePVs(1.5f);

        OnBoostChosen();
    }

    public void HealCharacters()
    {
        foreach (Hero hero in R.get.game.heroes)
        {
            hero.Heal(10);
        }


        OnBoostChosen();

    }

    public void FirerateBoostCharacters()
    {
        foreach (Hero hero in R.get.game.heroes)
        {
            hero.GetBoosterCrate(2f, false, 10);
        }


        OnBoostChosen();

    }

    public void ShieldBoostCharacters()
    {
        foreach (Hero hero in R.get.game.heroes)
        {
            hero.GetBoosterCrate(1, true, 10);
        }


        OnBoostChosen();

    }

    public void PauseWavess()
    {
        R.get.levelManager.level.PauseEnemies(10);

        OnBoostChosen();

    }

    public void GainMoreEXP()
    {
        R.get.game.currentXPMultiplier *= 1.5f;

        OnBoostChosen();
    }

    public void RevivePartner()
    {
        for(int i = 0; i < 2; i++)
        {
            if (R.get.game.heroes[i].dead) R.get.game.heroes[i].Revive();
        }

        OnBoostChosen();
    }


    void OnBoostChosen()
    {
        HideAmeliorationMenu();

        R.get.haptics.Haptic(HapticForce.Medium);

        R.get.game.StopPause();
    }


    public void SetPause()
    {
        Time.timeScale = 0;
    }

    public void StopPause()
    {
        Time.timeScale = 1;
    }

    public void GoToStartMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

}
