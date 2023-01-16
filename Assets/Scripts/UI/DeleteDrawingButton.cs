using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DeleteDrawingButton : MonoBehaviour
{
    [SerializeField] public int secondsBeforeDelete = 2;

    bool fingerOn = true;
    Color startColor;

    private void Awake()
    {
        startColor = GetComponent<Image>().color;
    }

    public void Show()
    {
        GetComponent<Image>().enabled = true;
    }

    public void Hide()
    {
        GetComponent<Image>().enabled = false;
    }

    public void GetOnButton()
    {
        fingerOn = true;
        StartCoroutine(WaitBeforeDeleteLineCoroutine());
    }

    public void Leave()
    {
        if (!Input.GetMouseButton(0)) return;
        fingerOn = false;

        ResetButton();

        R.get.game.controls.deleteLine = false;
    }

    private void Update()
    {
        if (fingerOn && Input.GetMouseButtonUp(0)) OnFingerUp();
    }

    public void OnFingerUp()
    {
        ResetButton();
    }

    void ResetButton()
    {
        transform.localScale = Vector3.one;
        GetComponent<Image>().color = startColor;
        DOTween.Kill("DeleteButton");
    }

    IEnumerator WaitBeforeDeleteLineCoroutine()
    {
        transform.DOScale(Vector3.one * 3f, secondsBeforeDelete).SetId("DeleteButton").SetUpdate(UpdateType.Normal, true);
        GetComponent<Image>().DOColor(Color.red, secondsBeforeDelete / 3f).SetDelay(secondsBeforeDelete * 0.6f).SetUpdate(UpdateType.Normal, true).SetId("DeleteButton");

        float t = 0;
        while (fingerOn && t <= secondsBeforeDelete)
        {
            Debug.Log("Preparing for delete" + t);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        if(t >= secondsBeforeDelete) R.get.game.controls.deleteLine = true;
    }



}
