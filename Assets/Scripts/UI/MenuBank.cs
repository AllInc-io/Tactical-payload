using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class MenuBank : MonoBehaviour
{
    [FoldoutGroup("Refs")] public RessourcePanel ressourcePanel;
    [FoldoutGroup("Prefabs")] public RectTransform ressourceBase;

    public void Init()
    {
        ressourcePanel.Init();
        UpdateGold();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateGold()
    {
        ressourcePanel.UpdateValue(R.get.score);
    }


    public void AnimateRessourcesGoingIntoBank(Vector3 from, int amount, Vector2 randomRange)
    {
        StartCoroutine(AnimRessource(from, amount, randomRange));
    }

    IEnumerator AnimRessource(Vector3 from, int nb, Vector2 randomRange)
    {

        int max = 30;
        if (nb > max) R.get.AddScore(nb - max);
        nb = max;
        for (int i = nb - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(0.06f);

            RectTransform ress = Instantiate(ressourceBase).GetComponent<RectTransform>();

            ress.pivot = new Vector2(0, 0);
            ress.anchorMin = new Vector2(0, 0);

            ress.SetParent(transform);



            Vector3 ressPos = U.WorldToUIPos(from, R.get.mainCamera, GetComponentInParent<CanvasScaler>());


            ress.gameObject.SetActive(true);

            ressPos.x += Random.Range(-randomRange.x, randomRange.x);
            ressPos.y += Random.Range(-randomRange.y, randomRange.y);
            ress.anchoredPosition = ressPos;

            ress.localScale = Vector3.zero;

            float randValue = Random.value * 0.35f;
            ress.DOScale(1f, 0.3f + randValue).SetEase(Ease.OutQuad);
            ress.DOJump(ressourcePanel.animAnchor.position, 0.5f, 1, 0.5f + randValue).SetEase(Ease.InSine).OnComplete(() => EndAnimRessource(ress));
        }


    }

    void EndAnimRessource(RectTransform ress)
    {
        ress.gameObject.SetActive(false);
        R.get.AddScore();
    }

}
