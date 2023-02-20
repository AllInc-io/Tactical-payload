using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;

public class MenuWin : MonoBehaviour
{

    [FoldoutGroup("Refs")] public RectTransform ressourceBase;
    [FoldoutGroup("Refs")] public RectTransform halo;
    [FoldoutGroup("Refs")] public RectTransform crown;
    [FoldoutGroup("Refs")] public RectTransform anchorRessource;
    [FoldoutGroup("Refs")] public TextMeshProUGUI textRating;
    [FoldoutGroup("Refs")] public TextMeshProUGUI textAmount;
    [FoldoutGroup("Refs")] public CharacterUnlockedPopup charaUnlockedPopup;

    int ressourceAdding;

    public void Init()
    {

        charaUnlockedPopup.Init();

    }

    public void Show(float delay)
    {

        gameObject.SetActive(true);

        StartCoroutine(ShowCoroutine(delay));
        /*if ((R.get.lastLevelFinished) % R.get.levelDesign.newCharacterEveryX == 0 && R.get.lastLevelFinished/R.get.levelDesign.newCharacterEveryX < R.get.levelDesign.possibleCharactersPrefabs.Length - 2)
        {
            Debug.Log("Unlocked new character");
            charaUnlockedPopup.gameObject.SetActive(true);
            charaUnlockedPopup.Show(R.get.levelDesign.possibleCharactersPrefabs[Mathf.FloorToInt((float)R.get.lastLevelFinished / R.get.levelDesign.newCharacterEveryX) + 2]);
        }*/
    }

    IEnumerator ShowCoroutine(float delay)
    {


        transform.localScale = Vector3.zero;

        int yardsTravelled = R.get.levelManager.level.payload.progression;

        textRating.text = yardsTravelled + "yards";
        textAmount.text = "+ " + yardsTravelled;

        R.get.AddToScore(yardsTravelled);


        yield return new WaitForSeconds(delay);


        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);


        StartCoroutine(AnimRessource(yardsTravelled));

        if (crown != null) crown.DOLocalMoveY(crown.transform.localPosition.y + 80f, 0.6f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        halo.DOLocalRotate(new Vector3(0f, 0f, 360f), 2.8f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);

    }

    IEnumerator AnimRessource(int nb)
    {



        int max = 30;
        if (nb > max) R.get.AddMoney(nb - max);
        nb = max;


        ressourceAdding = nb;


        yield return new WaitForSeconds(0.8f);

        for(int i=nb-1; i >= 0; i--)
        {
            yield return new WaitForSeconds(0.06f);

            RectTransform ress = Instantiate(ressourceBase).GetComponent<RectTransform>();
            ress.SetParent(transform);
    
            Vector3 ressPos = anchorRessource.position;
            ress.gameObject.SetActive(true);

            ressPos.x += Random.Range(-100f, 100f);
            ressPos.y += Random.Range(-100f, 100f);
            ress.position = ressPos;

            ress.localScale = Vector3.zero;
            ress.DOScale(2f, 0.4f).SetEase(Ease.OutQuad);

            float randValue = Random.value * 0.35f;
            ress.DOScale(1f, 0.45f).SetEase(Ease.OutQuad).SetDelay(0.4f);
            ress.DOJump(R.get.ui.menuBank.ressourcePanel.animAnchor.position, 0.5f, 1, 0.5f + randValue).SetEase(Ease.InSine).SetDelay(0.3f).OnComplete(() => EndAnimRessource(ress));

            
        }

        yield return new WaitForSeconds(0.5f);

        if(nb>=10)
            textRating.text = "Amazing!!";
        else if(nb>=7)
            textRating.text = "Great!";
        else if(nb>=3)
            textRating.text = "Nice!";
        else   
            textRating.text = "Good!";


        textRating.enabled = true;
        textRating.transform.localScale = Vector3.zero;
        textRating.transform.DOScale(1f, 0.45f).SetEase(Ease.OutBack);
    }

    void EndAnimRessource(RectTransform ress)
    {
        ress.gameObject.SetActive(false);
        R.get.AddMoney();
        ressourceAdding--;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ClickButtonContinue()
    {
        ReloadSceneSettings.clickedHomeAfterWinningLevel = true;
        SceneManager.LoadScene(1);
        R.get.AddMoney(ressourceAdding);
    }

}
