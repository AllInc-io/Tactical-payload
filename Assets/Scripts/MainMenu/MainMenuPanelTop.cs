using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MainMenuPanelTop : MonoBehaviour
{
    [FoldoutGroup("Refs")] public RessourcePanel ressourcePanel;

    public void Init()
    {
        ressourcePanel.Init();
        ressourcePanel.Show();
        ressourcePanel.UpdateValue(R.get.money);
    }


}
