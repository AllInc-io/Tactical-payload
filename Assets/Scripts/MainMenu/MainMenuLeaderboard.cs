using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLeaderboard : MonoBehaviour
{
    public List<LeaderboardItem> listItems;
    public ScrollRectEx scrollView;
    //public EnterYourNamePopup enterYourNamePopup;

    public void Init()
    {
        int playerRank = R.get.leaderboard.GetCurrentPlayerRank(R.get.score);
        int k = 7;

        if(playerRank-7<=0)
            k = playerRank-1;


        if(playerRank + 3 > R.get.leaderboard.leaderboard.Count)
        {
            k = 10 - (R.get.leaderboard.leaderboard.Count-playerRank);

            scrollView.verticalNormalizedPosition = 0f;
        }
            

        for(int i=0; i < listItems.Count; i++)
        {
            int currentRank = playerRank-(k-i);
            listItems[i].Init();

            if(currentRank == playerRank)
                listItems[i].SetupForPlayer();
            else if(currentRank >= 1 && currentRank <= R.get.leaderboard.leaderboard.Count)
                listItems[i].Setup(R.get.leaderboard.leaderboard[currentRank-1]);
        }

        //enterYourNamePopup.Init();
        
    }

    public void ShowEnterYourNamePopup()
    {
        //enterYourNamePopup.Show();
    }

    public void ChangeUsername(string username)
    {
        PlayerPrefs.SetString("Username", username);
        listItems.Find(item => item.backPlayer.activeInHierarchy).textName.text = username;
    }
}
