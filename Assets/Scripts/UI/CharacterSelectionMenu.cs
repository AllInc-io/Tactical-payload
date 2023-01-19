using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CharacterSelectionMenu : MonoBehaviour
{

    [SerializeField] CharacterSelectionButton listButtonPrefab;

    [SerializeField] CharacterSelectionSlot[] slots;
    [SerializeField] Material lockedCharaMat;
    public RectTransform buttonsListParent;
    public List<CharacterSelectionButton> buttonsList;
    [SerializeField] public Image listBG;
    public int currentlySelectedCharacterSlot = 0;

    [SerializeField] public Transform[] heroParents;

    List<int> fullSpots = new List<int>();

    string[] heroesSelected;

    public void Init()
    {
        buttonsList = new List<CharacterSelectionButton>();

        int unlockedCharactersAmount = Mathf.Clamp(Mathf.FloorToInt((float)R.get.lastLevelFinished / R.get.levelDesign.newCharacterEveryX) + 3, 3, R.get.levelDesign.possibleCharactersPrefabs.Length);
        for(int i = 0; i < R.get.levelDesign.possibleCharactersPrefabs.Length; i++)
        {
            CharacterSelectionButton button = Instantiate(listButtonPrefab, buttonsListParent);
            button.Init(R.get.levelDesign.possibleCharactersPrefabs[i], true);
            buttonsList.Add(button);
        }

        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].index = i;
        }

        SetListParentSize();
        buttonsListParent.parent.gameObject.SetActive(false);

        //inits the team with the 3 heroes we previously selected

        heroesSelected = new string[3];

        int max = 1;
        if (R.get.lastLevelFinished >= R.get.levelDesign.levelUnlockSecondChara - 1) max++;
        if (R.get.lastLevelFinished >= R.get.levelDesign.levelUnlockThirdChara - 1) max++;


        try
        {
            heroesSelected = PlayerPrefs.GetString("Team").Split(',');
            for (int i = 0; i < max; i++)
            {

                currentlySelectedCharacterSlot = i;
                ClickOnList(R.get.levelDesign.possibleCharactersPrefabs.Where(hero => hero.name.Equals(heroesSelected[i])).FirstOrDefault());

            }
        }
        catch(System.Exception e)
        {
            heroesSelected = new string[3];
            Debug.Log("Exception occured when trying to restore previous team. Details : " + e.Message);
            for (int i = 0; i < 3; i++)
            {
                currentlySelectedCharacterSlot = i;
                ClickOnList(R.get.levelDesign.possibleCharactersPrefabs[i]);
                heroesSelected[i] = R.get.levelDesign.possibleCharactersPrefabs[i].heroName;
                
            }
        }

        if (R.get.lastLevelFinished < R.get.levelDesign.levelUnlockSecondChara - 1)
        {
            slots[1].SetLocked(R.get.levelDesign.levelUnlockSecondChara);
            R.get.game.heroes[1] = Instantiate(R.get.levelDesign.possibleCharactersPrefabs[1], heroParents[1]);
            R.get.game.heroes[1].transform.localPosition = Vector3.zero;
            R.get.game.heroes[1].transform.localScale = Vector3.one;
            R.get.game.heroes[1].transform.localRotation = Quaternion.Euler(Vector3.zero);
            R.get.game.heroes[1].lifeCircle.gameObject.SetActive(false);
            R.get.game.heroes[1].gun.gameObject.SetActive(false);
            R.get.game.heroes[1].animator.SetLayerWeight(1, 0);

            

            foreach (SkinnedMeshRenderer renderer in heroParents[1].GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.material = lockedCharaMat;
            }
        }
        
    }


    void SetListParentSize()
    {

        float height = 550;
        int activeButtonsCount = buttonsList.Count;
        buttonsListParent.sizeDelta = new Vector2(buttonsListParent.sizeDelta.x, activeButtonsCount * height);
        buttonsListParent.anchoredPosition = new Vector2(buttonsListParent.anchoredPosition.x, activeButtonsCount * height / 2f);
    }

    public void ChangeSelectedChara(int value)
    {
        currentlySelectedCharacterSlot = value;
        buttonsListParent.parent.gameObject.SetActive(true);

        //listBG.gameObject.SetActive(true);

    }

    public void ClickOnList(Hero chara, bool withHaptic = true)
    {

        //Vector3 pos = R.get.levelManager.level.currentZone.entry.position - Vector3.forward * currentlySelectedCharacterSlot * 2f;
        //Vector3 scale = Vector3.one * 0.75f;
        if (R.get.game.heroes[currentlySelectedCharacterSlot] != null)
        {
            buttonsList.Where(button => button.chara.heroName == R.get.game.heroes[currentlySelectedCharacterSlot].heroName).FirstOrDefault().SetUnselected();
            Destroy(R.get.game.heroes[currentlySelectedCharacterSlot].gameObject);
        }

        heroesSelected[currentlySelectedCharacterSlot] = chara.heroName;

        R.get.game.heroes[currentlySelectedCharacterSlot] = Instantiate(chara, heroParents[currentlySelectedCharacterSlot]);
        R.get.game.heroes[currentlySelectedCharacterSlot].transform.localPosition = Vector3.zero;
        R.get.game.heroes[currentlySelectedCharacterSlot].transform.localScale = Vector3.one;
        R.get.game.heroes[currentlySelectedCharacterSlot].transform.localRotation = Quaternion.Euler(Vector3.zero);
        R.get.game.heroes[currentlySelectedCharacterSlot].lifeCircle.gameObject.SetActive(false);

        buttonsList.Where(button => button.chara == chara).FirstOrDefault().SetSelected();

        buttonsListParent.parent.gameObject.SetActive(false);

        InitSlot(currentlySelectedCharacterSlot);

        //if (fullSpots.Count == slots.Length) R.get.ui.menuStart.SetButtonStartPlayable();

        SetListParentSize();

        //listBG.gameObject.SetActive(false);

        SaveTeamInPlayerPrefs();

        if (withHaptic) R.get.haptics.Haptic(HapticForce.Selection);

    }

    void InitSlot(int slot)
    {
        //slots[slot].transform.Find("bg").gameObject.SetActive(true);
        //slots[slot].transform.Find("emptyBG").gameObject.SetActive(false);

        //slots[slot].transform.Find("bg").GetComponent<Image>().color = R.get.game.heroes[slot].lineColor;

        slots[slot].Init(R.get.game.heroes[slot]);
        //slots[slot].charaName.text = R.get.game.heroes[slot].heroName;
        //slots[slot].transform.Find("Level").GetComponentInChildren<TextMeshProUGUI>().text = "lvl " + (PlayerPrefs.GetInt("Level" + R.get.game.heroes[slot].heroName) + 1);

        if (!fullSpots.Contains(slot)) fullSpots.Add(slot);
    }

    void SaveTeamInPlayerPrefs()
    {
        string result = "";
        for (int i = 0; i < 3; i++)
        {
            result += heroesSelected[i] + ',';

        }
        PlayerPrefs.SetString("Team", result);
    }

    //will be called after spending money in menu, to check that buttons are properly greyed out if now unaffordable
    public void CheckAllScoreUpdateButton()
    {
        foreach(CharacterSelectionSlot slot in slots)
        {
            //slot.CheckScoreForUpdateButton();
        }
        foreach(CharacterSelectionButton button in GetComponentsInChildren<CharacterSelectionButton>())
        {
            //button.CheckScoreForUpdateButton();
        }
    }

}
