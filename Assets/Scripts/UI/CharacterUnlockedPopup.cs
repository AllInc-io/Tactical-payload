using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUnlockedPopup : PopupBase
{

    public TextMeshProUGUI charaName;

    public Image icon;


    public TextMeshProUGUI speedText;
    public TextMeshProUGUI firerateText;
    public TextMeshProUGUI damageText;

    public Hero chara;

    int level;
    int upgradePrice;

    public void Show(Hero chara)
    {
        base.Show();
        this.chara = chara;
        charaName.text = chara.heroName;

        icon.sprite = chara.icon;



        UpdateValues(PlayerPrefs.GetInt("Level" + chara.heroName));

    }

    public void UpdateValues(int level)
    {

        speedText.text = "Speed: \n " + chara.speed * R.get.levelDesign.EvaluateCharaSpeedMultiplier(level);
        firerateText.text = "Firerate: \n" + chara.startGunData.bulletsPerSecond * R.get.levelDesign.EvaluateCharaFirerateMultiplier(level);
        damageText.text = "Damage: \n" + chara.startGunData.damagePerBullet * R.get.levelDesign.EvaluateCharaDamageMultiplier(level);
    }
}
