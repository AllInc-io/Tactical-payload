using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionSlot : MonoBehaviour
{

    public TextMeshProUGUI charaName;
    //public TextMeshProUGUI charaLevelText;

    //public RectTransform upgradeButton;
    //public TextMeshProUGUI upgradePriceText;

    [SerializeField] Transform locked;
    [SerializeField] TextMeshProUGUI unlockAtLevelText;

    public Image bg;

    public Hero chara;

    int level;
    int upgradePrice;

    public int index;

    public void Init(Hero chara)
    {
        this.chara = chara;
        charaName.text = chara.heroName.ToUpper();



        //if (charaLevelText != null) charaLevelText.text = "level " + (PlayerPrefs.GetInt("Level" + chara.heroName) + 1).ToString();


        level = PlayerPrefs.GetInt("Level" + chara.heroName);
        upgradePrice = R.get.levelDesign.EvaluateUpgradePrice(level);

        /*if (upgradeButton != null)
        {
            if (level + 1 >= R.get.levelDesign.characterMaxLevel) upgradeButton.gameObject.SetActive(false);

            upgradePriceText.text = upgradePrice.ToString();
            CheckScoreForUpdateButton();
        }*/

    }


    public void OnClickChangeButton()
    {
        R.get.startMenu.characterSelectionMenu.ChangeSelectedChara(index);
    }

    public void TryPurchaseLevelUp()
    {
        if (R.get.score >= upgradePrice)
        {
            R.get.AddScore(-upgradePrice);
            OnLevelUp();
        }
    }

    public void OnLevelUp()
    {

        level++;
        PlayerPrefs.SetInt("Level" + chara.heroName, level);

        //level up fx
        R.get.startMenu.characterSelectionMenu.heroParents[index].GetComponentInChildren<ParticleSystem>().Play();

        /*if (level + 1 >= R.get.levelDesign.characterMaxLevel) upgradeButton.gameObject.SetActive(false);
        upgradePrice = R.get.levelDesign.EvaluateUpgradePrice(level);
        upgradePriceText.text = upgradePrice.ToString();

        if (charaLevelText != null) charaLevelText.text = "level " + (level + 1);

        if (upgradePrice > R.get.score) upgradeButton.Find("Locked").gameObject.SetActive(true);
        else upgradeButton.Find("Locked").gameObject.SetActive(false);*/
    }

    public void CheckScoreForUpdateButton()
    {

        /*if (upgradePrice > R.get.score) upgradeButton.Find("Locked").gameObject.SetActive(true);
        else upgradeButton.Find("Locked").gameObject.SetActive(false);*/

    }

    public void SetLocked(int unlockLevel)
    {
        locked.gameObject.SetActive(true);
        unlockAtLevelText.text = "Unlock at lvl " + unlockLevel;
    }
}
