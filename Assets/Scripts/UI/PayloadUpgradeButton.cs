using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PayloadUpgradeButton : MonoBehaviour
{


    public TextMeshProUGUI charaName;
    public TextMeshProUGUI valueText;
    //public TextMeshProUGUI charaLevelText;

    public RectTransform upgradeButton;
    public TextMeshProUGUI upgradePriceText;

    [SerializeField] Transform locked;


    int baseValue;
    int basePrice;

    int level;
    int upgradePrice;


    string characteristics;

    public void Init(int baseValue, int basePrice, string characteristics, int level)
    {

        this.level = level;


        //if (charaLevelText != null) charaLevelText.text = "level " + (PlayerPrefs.GetInt("Level" + chara.heroName) + 1).ToString();

        this.baseValue = baseValue;
        this.basePrice = basePrice;

        valueText.text = (characteristics.Equals("PayloadLife") ?  R.get.levelDesign.GetPayloadLife(level) + " hp" : (R.get.levelDesign.GetPayloadSpeed(level) * 5f) + " mph").ToString();
        upgradePrice = Mathf.RoundToInt(basePrice * Mathf.Pow(2, level));
        upgradePriceText.text = upgradePrice.ToString();

        CheckScoreForUpdateButton();

        /*if (upgradeButton != null)
        {
            if (level + 1 >= R.get.levelDesign.characterMaxLevel) upgradeButton.gameObject.SetActive(false);

            upgradePriceText.text = upgradePrice.ToString();
            CheckScoreForUpdateButton();
        }*/

        this.characteristics = characteristics;
    }



    public void TryPurchaseLevelUp()
    {
        if (R.get.money >= upgradePrice)
        {
            R.get.AddMoney(-upgradePrice);
            OnLevelUp();
        }
    }

    public void OnLevelUp()
    {

        level++;
        PlayerPrefs.SetInt(characteristics + "Level", level);



        valueText.text = (characteristics.Equals("PayloadLife") ? R.get.levelDesign.GetPayloadLife(level) + " hp" : (R.get.levelDesign.GetPayloadSpeed(level) * 5f) + " mph").ToString();
        upgradePrice = Mathf.RoundToInt(basePrice * Mathf.Pow(2, level));

        upgradePriceText.text = upgradePrice.ToString();

        CheckScoreForUpdateButton();


        if (upgradePrice > R.get.money) upgradeButton.Find("Locked").gameObject.SetActive(true);
        else upgradeButton.Find("Locked").gameObject.SetActive(false);
    }

    public void CheckScoreForUpdateButton()
    {

        if (upgradePrice > R.get.money) upgradeButton.Find("Locked").gameObject.SetActive(true);
        else upgradeButton.Find("Locked").gameObject.SetActive(false);

    }


}

