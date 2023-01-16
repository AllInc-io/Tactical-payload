using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuSettings : MonoBehaviour
{

    public ToggleButton haptics;
    public ToggleButton music;
    public ToggleButton sfx;
    public TextMeshProUGUI textVersion;

    public void Init()
    {
        haptics.Init();
        music.Init();
        sfx.Init();

        if (PlayerPrefs.GetInt("HasHaptic") == 1)
            haptics.SetOnInstant();

        /*
        if(R.get.hasMusic)
            music.SetOn();

        if (R.get.hasSfx)
            sfx.SetOn();
        */

        textVersion.text = "Game Version: " + Application.version;
    }

    public void OnClickHaptics()
    {
        HapticManager.instance.ChangeHaptic();

        if(HapticManager.instance.HapticActived)
            haptics.SetOn();
        else
            haptics.SetOff();
    }


    public void OnClickMusic()
    {
        
        PlayerPrefs.SetInt("HasMusic", R.get.hasMusic ? 0 : 1);
        R.get.hasMusic = !R.get.hasMusic;

        if(!R.get.hasMusic)
        {
            //SoundManager.get.TurnOff();
            music.SetOff();
        }
        else
        { 
            //SoundManager.get.TurnOn();
            music.SetOn();
        }
        
    }

    public void OnClickSFX()
    {
        PlayerPrefs.SetInt("HasSFX", R.get.hasSFX ? 0 : 1);
        R.get.hasSFX = !R.get.hasSFX;

        if (R.get.hasSFX)
            sfx.SetOn();
        else
            sfx.SetOff();
        
    }


/*
    public void OnClickGDPR()
    {
        VoodooSauce.ShowGDPRSettings();
    }

    public void OnClickRestorePurchase()
    {
        R.get.mainMenu.fullscreenPopup.Show(Localization.GetTranslation("Shop.AttemptingRestorePurchase"));

        VoodooSauce.RestorePurchases((response) => {
            if (response == RestorePurchasesResult.SuccessRestorationProcess)
            {
                ShowPurchasesRestoredPopup(true);
            }
            else
            {
                ShowPurchasesRestoredPopup(false);
            }
        });
    }

    private void ShowPurchasesRestoredPopup(bool success = false)
    {
        StartCoroutine(AnimRestorePurchase(success));
    }

    IEnumerator AnimRestorePurchase(bool success)
    {
        R.get.mainMenu.fullscreenPopup.UpdateText(success ? "Purchase restored successfully" : "Restore purchase failed");

        yield return new WaitForSeconds(1f);

        R.get.mainMenu.fullscreenPopup.Hide();
    }
*/
    
}
