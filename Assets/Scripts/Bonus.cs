using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bonus : MonoBehaviour
{
    [SerializeField] bool turnsInvulnerable = false;
    [SerializeField] int currencyAmount = 0;
    [SerializeField] int healAmount = 0;
    [SerializeField] float fireRateMultiplier = 1;
    [SerializeField] float bonusDuration = 10;
    [SerializeField] float lifetime = 5f;
    [SerializeField] bool levelsUp;

    float liveCounter;

    bool beingDestroyed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            R.get.haptics.Haptic(HapticForce.Medium);
            Hero hero = other.GetComponent<Hero>();
            if(fireRateMultiplier > 1 || turnsInvulnerable) hero.GetBoosterCrate(fireRateMultiplier, turnsInvulnerable, bonusDuration);
            if(healAmount > 0) hero.Heal(healAmount);
            if (currencyAmount > 0) R.get.ui.menuBank.AnimateRessourcesGoingIntoBank(transform.position, currencyAmount, new Vector2(Screen.width *0.05f, Screen.height * 0.05f));
            if (levelsUp) hero.LevelUp();
            //R.get.levelManager.level.currentZone.cratesPickedUp ++;
            Destroy(this.gameObject);
        }
    }

    void Update()
    {

        //if (liveCounter > 2 && lifetime - Time.deltaTime - liveCounter <= 2) transform.DOLocalMoveY(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);

        liveCounter += Time.deltaTime;
        
        if (liveCounter >= lifetime && !beingDestroyed) Destroy();

    }

    private void Destroy()
    {
        beingDestroyed = true;

        transform.DOKill();
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => Destroy(this.gameObject));
    }

    public void Drop(Vector3 pos)
    {
        transform.DOMove(pos, 1f).SetEase(Ease.InCubic);
    }

   
}
