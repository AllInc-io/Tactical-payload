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
    [SerializeField] RectTransform waveIncomingArrowsParent;
    [SerializeField] RectTransform waveIncomingArrowU;
    [SerializeField] RectTransform waveIncomingArrowR;
    [SerializeField] RectTransform waveIncomingArrowD;
    [SerializeField] RectTransform waveIncomingArrowL;

    [SerializeField] public ZoneFinishedPopup zoneFinishedPopup;

    public DeleteDrawingButton deleteDrawingButton;

    public GameObject dotProgressionPrefab;
    public Image fillProgression;

    private List<GameObject> dots;

    [SerializeField] Image xpBar;

    public void Init()
    {
        waveIncomingText.transform.localScale = Vector3.zero;
        //waveIncomingArrowsParent.localScale = Vector3.zero;


        InitProgression();
        zoneFinishedPopup.Init();

        HideTutorialText();
        HidePointer();
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

        waveIncomingText.text = "Wave incoming : " + index + "/" + totalAmount;
        Vector3 refPos = R.get.levelManager.level.payload.transform.position;

        //waveIncomingArrow.up = new Vector3(direction.x, direction.z, 0);
        //waveIncomingArrow.anchoredPosition = new Vector2(Mathf.RoundToInt(direction.x) * Screen.width * 0.4f, Mathf.RoundToInt(direction.z) * Screen.height * 0.4f);
        Transform arrow;
        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            arrow = direction.x >= refPos.x ? waveIncomingArrowR : waveIncomingArrowL; 
        }
        else
        {
            arrow = direction.z >= refPos.z ? waveIncomingArrowU : waveIncomingArrowD;
        }
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

    public void SetXPBar(float fill)
    {
        xpBar.DOKill();
        if (fill > xpBar.fillAmount) xpBar.DOFillAmount(fill, 0.3f);
        else xpBar.fillAmount = fill;
    }

}
