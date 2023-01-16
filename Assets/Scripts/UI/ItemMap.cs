using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class ItemMap : MonoBehaviour
{
    [FoldoutGroup("Refs")] public Image backgroundPlayed;
    [FoldoutGroup("Refs")] public Image backgroundLocked;
    [FoldoutGroup("Refs")] public Image backgroundCurrent;
    [FoldoutGroup("Refs")] public Image backgroundBossPlayed;
    [FoldoutGroup("Refs")] public Image backgroundBossLocked;
    [FoldoutGroup("Refs")] public Image backgroundBossCurrent;
    [FoldoutGroup("Refs")] public Image bossCartel;
    [FoldoutGroup("Refs")] public Image lockIcon;
    [FoldoutGroup("Refs")] public Image starCartel;
    [FoldoutGroup("Refs")] public Image path;
    [FoldoutGroup("Refs")] public Transform cartelItem;
    [FoldoutGroup("Refs")] public GameObject star;
    [FoldoutGroup("Refs")] public TextMeshProUGUI text;
    [FoldoutGroup("Refs")] public RectTransform rectTransform;
    
    RectTransform pathRect;
    bool isBoss;
    bool hasStar;
    public int ID { get; private set; }
    public LevelMapState CurrentState { get; private set; }

    public void Init()
    {

    }

    public void Setup(int _ID, LevelMapState state, bool _isBoss, bool _hasStar)
    {
        Reset();

        isBoss = _isBoss;
        hasStar = _hasStar;
        ID = _ID;
        text.text = "" + _ID;

        if(hasStar && !isBoss)
            star.SetActive(true);

        pathRect = path.transform.parent.GetComponent<RectTransform>();

        switch(state)
        {
            case LevelMapState.Played: SetPlayed(); break;
            case LevelMapState.Current: SetCurrent(); break;
            case LevelMapState.Locked: SetLocked(); break;
        }
    }
    public void SetPlayed()
    {
        if(isBoss)
        {
            bossCartel.enabled = true;
            backgroundBossPlayed.enabled = true;
        }
        else
            backgroundPlayed.enabled = true;

        text.color = Color.white;
        path.fillAmount = 1f;
        lockIcon.enabled = false;
        CurrentState = LevelMapState.Played;
    }

    public void SetCurrent()
    {
        if(isBoss)
        {
            bossCartel.enabled = true;
            backgroundBossCurrent.enabled = true;
        }
        else
            backgroundCurrent.enabled = true;

        text.color = Color.white;
        path.fillAmount = 0f;
        lockIcon.enabled = false;
        CurrentState = LevelMapState.Current;
    }

    public void SetLocked()
    {
        if(isBoss)
        {
            bossCartel.enabled = true;
            backgroundBossLocked.enabled = true;
        }
        else
            backgroundLocked.enabled = true;

        lockIcon.enabled = true;
        path.fillAmount = 0f;
        CurrentState = LevelMapState.Locked;
    }

    public void FlashToCurrent(Action _OnAnimationCompleted=null)
    {
        StartCoroutine(AnimFlashToCurrent(() =>
        {
            _OnAnimationCompleted?.Invoke();
        }));
    }

    IEnumerator AnimFlashToCurrent(Action _OnAnimationCompleted)
    {   
        lockIcon.enabled = false;
        cartelItem.DOPunchScale(Vector3.one*0.25f, 0.7f, 5, 0.5f);

        yield return new WaitForSeconds(0.25f);

        SetCurrent();
        
        if(_OnAnimationCompleted != null)
            _OnAnimationCompleted?.Invoke();
    }

    void Reset()
    {
        star.SetActive(false);
        lockIcon.enabled = false;
        bossCartel.enabled = false;
        backgroundPlayed.enabled = false;
        backgroundLocked.enabled = false;
        backgroundCurrent.enabled = false;
        backgroundBossPlayed.enabled = false;
        backgroundBossLocked.enabled = false;
        backgroundBossCurrent.enabled = false;
    }

    public void ForceEmptyPath()
    {
        path.fillAmount = 0f;
    }

     public void TweenFillPath()
    {
        path.DOFillAmount(1f, 0.9f).SetEase(Ease.OutQuad);
    }


}

public enum LevelMapState
{
    Played,
    Current,
    Locked
}

