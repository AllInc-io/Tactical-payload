using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenManager : MonoBehaviour {
	public int mainSceneIndex;
	public int menuSceneIndex;
	public float waitTime;

	bool voodooInitDone;

	void Awake() 
	{
		//VoodooSauce.SubscribeOnInitFinishedEvent(OnVoodooInitFinished);
		voodooInitDone = true;

		if (PlayerPrefs.GetInt("LastLevelFinished") > 0)
		{
			StartCoroutine(LoadingScene(menuSceneIndex));
		}
		else
		{
			StartCoroutine(LoadingScene(mainSceneIndex));
		}
		//loadingText.DOFade(0, 1.0f).SetLoops(-1, LoopType.Yoyo);
		//SceneManager.LoadScene(1);
		/*
		if (PlayerPrefs.GetInt("HasInit") == 0)
		{
			Debug.Log("RESTART PLAYERPREF");
			//HapticManager.instance.HapticActived = true;
			//PlayerPrefs.SetInt("HasHaptic", HapticManager.instance.HapticActived?1:0);
			PlayerPrefs.SetInt("HasInit", 1);
			PlayerPrefs.SetInt("HasTilt", 1);
			PlayerPrefs.SetInt("HasHaptic", 1);
			PlayerPrefs.SetInt("HasMusic", 1);
			PlayerPrefs.SetInt("HasSFX", 1);
			PlayerPrefs.SetInt("DebugFlow", 0);
			PlayerPrefs.SetInt("Bank", 0);
			PlayerPrefs.SetString("Username", "Player");
			PlayerPrefs.SetInt("Skin", 0);
		}*/
	}

	IEnumerator LoadingScene(int index)
	{
		Debug.Log("Before Wait");
		yield return new WaitForSeconds(waitTime);
		Debug.Log("After Wait async");


		float timer = 0;
		//SceneManager.LoadScene(1);
		AsyncOperation async = SceneManager.LoadSceneAsync(index);
		async.allowSceneActivation = false;
#if UNITY_EDITOR
		while (!voodooInitDone)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		async.allowSceneActivation = true;
#else
		while ((!voodooInitDone && timer < 5) || !Voodoo.Tiny.Sauce.Privacy.PrivacyManager.ConsentReady)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		async.allowSceneActivation = true;
#endif
	}

	public void OnVoodooInitFinished()
	{
		Debug.Log("VOODOO LOADED");
		voodooInitDone = true;
	}
}
