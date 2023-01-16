using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class ZoneFinishedPopup : PopupBase
{

    [SerializeField, FoldoutGroup("Refs")] TextMeshProUGUI zombiesKilledText;
    [SerializeField, FoldoutGroup("Refs")] TextMeshProUGUI cratesPickedUpText;
    [SerializeField, FoldoutGroup("Refs")] TextMeshProUGUI moneyWonText;


    [SerializeField, FoldoutGroup("Descriptions")] string zombiesKilledDescription = "Zombies killed : ";
    [SerializeField, FoldoutGroup("Descriptions")] string cratesPickedUpDescription = "Crates picked up : ";
    [SerializeField, FoldoutGroup("Descriptions")] string moneyWonDescription = "Money won : ";


    public void Show(int zombiesKilled, int cratesPickedUp, int monewWon)
    {
        base.Show();
        zombiesKilledText.text = zombiesKilledDescription + zombiesKilled;
        cratesPickedUpText.text = cratesPickedUpDescription + cratesPickedUp;
        moneyWonText.text = moneyWonDescription + monewWon;

        //R.get.ui.menuBank.AnimateRessourcesGoingIntoBank(R.get.levelManager.level.currentZone.transform.position, R.get.levelDesign.moneyWonPerZombieKilled * zombiesKilled, new Vector2(10, 10));
    }


}
