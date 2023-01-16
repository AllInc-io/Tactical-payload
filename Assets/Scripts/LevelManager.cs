using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public Level level;
    public List<Transform> levels;
    
    public void Init()
    {
        int currentLevel = R.get.lastLevelFinished == 0 ? 0 : R.get.lastLevelFinished % ((levels.Count - 1)) + 1;

        

        if(transform.childCount > 0)
        {
            // si un level debug en child
            level = transform.GetChild(0).GetComponent<Level>();
        }
        else
        {
            // si pas de level debug
            level = Instantiate(levels[currentLevel]).GetComponent<Level>();
            level.transform.parent = transform;
            level.transform.position = Vector3.zero;
            level.transform.rotation = Quaternion.identity;
            level.transform.localScale = Vector3.one;
        }

        level.Init();
    }
}
