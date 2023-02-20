using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionButton : MonoBehaviour
{

    public TextMeshProUGUI charaName;
    //public TextMeshProUGUI charaLevelText;
    public Image icon;
    //public RectTransform upgradeButton;
    //public TextMeshProUGUI upgradePriceText;
    [SerializeField] RectTransform locked;
    [SerializeField] TextMeshProUGUI lockedText;
    [SerializeField] Image lockedIcon;
    [SerializeField] TextMeshProUGUI unlockedLevelText;
    [SerializeField] RectTransform selected;
    [SerializeField] Button selectButton;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI firerateText;
    public TextMeshProUGUI damageText;
    public Image bg;

    public Hero chara;

    int level;
    int upgradePrice;

    public void Init(Hero chara, bool unlocked)
    {

        this.chara = chara;

        if(unlocked)
        {
            charaName.text = chara.heroName;
            bg.gameObject.SetActive(true);
            //bg.color = chara.lineColor;
            icon.sprite = chara.icon;

            //if (charaLevelText != null) charaLevelText.text = (PlayerPrefs.GetInt("Level" + chara.heroName) + 1).ToString();

            UpdateValues(PlayerPrefs.GetInt("Level" + chara.heroName));

            level = PlayerPrefs.GetInt("Level" + chara.heroName);
            upgradePrice = R.get.levelDesign.EvaluateUpgradePrice(level);

            /*if (upgradeButton != null)
            {
                if (level + 1 >= R.get.levelDesign.characterMaxLevel) upgradeButton.gameObject.SetActive(false);

                upgradePriceText.text = upgradePrice.ToString();
            }*/


            CheckScoreForUpdateButton();
        }
        else
        {
            locked.gameObject.SetActive(true);
            lockedText.text = chara.heroName;
            lockedIcon.sprite = chara.icon;
            lockedIcon.color = Color.black;

            selectButton.gameObject.SetActive(false);
            unlockedLevelText.text = "Travel " + chara.scoreUnlock + " yards to unlock";
        }

    }

    public void SetSelected()
    {
        selected.gameObject.SetActive(true);
        selected.GetComponentInChildren<TextMeshProUGUI>().text = chara.heroName;
        selectButton.gameObject.SetActive(false);
        selectButton.transform.Find("Locked").gameObject.SetActive(true);

        
    }

    public void SetUnselected()
    {
        selected.gameObject.SetActive(false);
        selectButton.gameObject.SetActive(true);
        selectButton.transform.Find("Locked").gameObject.SetActive(false);
    }

    public void OnClick()
    {
        R.get.startMenu.characterSelectionMenu.ClickOnList(chara);
    }

    public void CheckScoreForUpdateButton()
    {

        /*if (upgradePrice > R.get.score) upgradeButton.Find("Locked").gameObject.SetActive(true);
        else upgradeButton.Find("Locked").gameObject.SetActive(false);*/

    }


    public void TryPurchaseLevelUp()
    {
        if(R.get.money >= upgradePrice)
        {
            R.get.AddMoney(-upgradePrice);
            OnLevelUp();
        }
    }

    public void OnLevelUp()
    {

        level++;
        PlayerPrefs.SetInt("Level" + chara.heroName, level);

        UpdateValues(level);

        /*
        if (level + 1 >= R.get.levelDesign.characterMaxLevel) upgradeButton.gameObject.SetActive(false);
        upgradePrice = R.get.levelDesign.EvaluateUpgradePrice(level);
        upgradePriceText.text = upgradePrice.ToString();

        if (charaLevelText != null) charaLevelText.text = "Level " + (level + 1);
        */

    }

    public void UpdateValues(int level)
    {
        //if(charaLevelText != null) charaLevelText.text = "Level " + (level + 1).ToString();

        speedText.text = "Speed : " + chara.speed * R.get.levelDesign.EvaluateCharaSpeedMultiplier(level);
        firerateText.text = "Firerate : " + chara.startGunData.bulletsPerSecond * R.get.levelDesign.EvaluateCharaFirerateMultiplier(level);
        damageText.text = "Damage : " + chara.startGunData.damagePerBullet * R.get.levelDesign.EvaluateCharaDamageMultiplier(level);
    }
}
