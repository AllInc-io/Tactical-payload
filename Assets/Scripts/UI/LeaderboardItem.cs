using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardItem : MonoBehaviour
{
    public GameObject backNPC;
    public GameObject backPlayer;
    public TextMeshProUGUI textRank;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textName;

    public Image editable;
    public Image flag;

    public void Init()
    {

    }

    
    public void Setup(FakeUser fakeUser)
    {

        textRank.text = "" + fakeUser._rank;
        textScore.text = "" + fakeUser._score;
        textName.text = "" + fakeUser._username;


        flag.sprite = fakeUser._flag;
        backNPC.SetActive(true);
        backPlayer.SetActive(false);
        editable.gameObject.SetActive(false);
    }

    public void SetupForPlayer()
    {
        textRank.text = "" + R.get.leaderboard.GetCurrentPlayerRank(R.get.score);
        textScore.text = "" + R.get.score;
        textName.text = "YOU";

        backNPC.SetActive(false);
        backPlayer.SetActive(true);
        editable.gameObject.SetActive(true);
    }

    /*
    public void OpenEditNamePopup()
    {
        R.get.mainMenu.leaderboard.enterYourNamePopup.gameObject.SetActive(true);
        R.get.mainMenu.leaderboard.ShowEnterYourNamePopup();
    }
    */



}
