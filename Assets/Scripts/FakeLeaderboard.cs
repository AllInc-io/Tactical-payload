using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Linq;

[CreateAssetMenu]
public class FakeLeaderboard : ScriptableObject
{

    public List<Sprite> usableFlags;

    public string usernames;
    public char usernamesSplitChar = ' ';
    public int scoreBest = 10000;
    public  int amountOfPeople = 500;

    public AnimationCurve scoreProgressCurve;
    
    public List<FakeUser> leaderboard = new List<FakeUser>();

    public int currentPlayerRank = 0;


    //returns the current player rank, which is the amount of people - index in the list
    public int GetCurrentPlayerRank(int bank)
    {
        FakeUser previousUser = leaderboard[amountOfPeople -1];
        if (bank < leaderboard[amountOfPeople - 1]._score) return amountOfPeople;
        for(int i = 0; i < amountOfPeople ; i++)
        {
            FakeUser user = leaderboard[i];
            if (user._score <= bank && previousUser._score >= bank)
            {
                return user._rank;
            }
            previousUser = user;
        }
        return 1;
    }


#if UNITY_EDITOR

    [Button(ButtonSizes.Large)]
    public void GenerateFakeUsers()
    {
        leaderboard.Clear();

        float diff = (float)scoreBest / (float)amountOfPeople;
        string[] usernamesSplit = usernames.Split(usernamesSplitChar);

        for (int i = 0; i < amountOfPeople; i++)
        {
            
            int nextScore =  (i+1) * ((i+1) / 100) + (i + 1);
            //int score = Mathf.FloorToInt((float)scoreBest / (float)amountOfPeople * i + Random.value * diff); //add a little random so the score are not too obviously fake


            //int score = Mathf.FloorToInt(Mathf.Sqrt(((float)scoreBest / (float)amountOfPeople) * (float) i)); //add a little random so the score are not too obviously fake
            int score = i * (i / (100 + i)) + i;
            score += Mathf.FloorToInt(Random.value * (nextScore - score));

            score = Mathf.FloorToInt(scoreProgressCurve.Evaluate(1 - (float) i / (float) amountOfPeople) * (float)scoreBest) + Random.Range(0,2);
            FakeUser user = new FakeUser(usernamesSplit[Random.Range(0,usernamesSplit.Length)]+(Random.Range(0,5)==0?""+Random.Range(0,999):""), usableFlags[Mathf.FloorToInt(Random.value * (float)usableFlags.Count)], score, i + 1);
            leaderboard.Add(user);
        }
        leaderboard = leaderboard.OrderByDescending(user => user._score).ToList();


    }

#endif
}

[System.Serializable]
public class FakeUser
{
    public FakeUser(string name, Sprite flag, int score, int rank)
    {
        _username = name;
        _flag = flag;
        _score = score;
        _rank = rank;
    }
    public string _username;
    public Sprite _flag;
    public int _score;
    public int _rank;
}