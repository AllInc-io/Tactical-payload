using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CollectibleCoin : MonoBehaviour
{
    void Awake()
    {
        transform.DOLocalRotate(new Vector3(0f, 360f, 0f), 2.8f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            DOTween.Kill(this.transform);
            R.get.AddMoney(10);
            Destroy(this.gameObject);
        }
    }




}
