using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofelt.NiceVibrations;

public enum HapticForce { Selection, Success, Warning, Failure, Light, Medium, Heavy };

public class HapticManager : MonoBehaviour 
{
	public static HapticManager instance = null;

	private float vibrationTimer;

	private bool hapticActived;
	public bool HapticActived 
	{
		get { return hapticActived; }
	}

	public void Init() 
	{
		if (instance != null && instance != this) 
		{
			Destroy(gameObject);
			return;
		} 
		else 
		{
			instance = this;
		}
	
#if UNITY_IOS
		//MMNViOS.iOSInitializeHaptics();
#endif

		hapticActived = (PlayerPrefs.GetInt("HasHaptic")!=0);
	}

	public void Haptic(HapticForce force) {
		if (hapticActived && CanVibrate()) {
			switch (force) {
				case HapticForce.Selection:
				HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
				break;

				case HapticForce.Success:
					HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
				break;

				case HapticForce.Warning:
					HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
				break;

				case HapticForce.Failure:
					HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
				break;

				case HapticForce.Light:
					HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
				break;

				case HapticForce.Medium:
					HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
				break;

				case HapticForce.Heavy:
					HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
				break;
			}
			Vibrate();
		}
	}

	public void ChangeHaptic()
	{
		hapticActived = !hapticActived;
		PlayerPrefs.SetInt("HasHaptic", HapticManager.instance.HapticActived?1:0);
	}

	bool CanVibrate() 
	{
		return vibrationTimer <= 0.0f;
	}

	public void Vibrate() {
#if UNITY_ANDROID
		vibrationTimer = .1f;
#else
		vibrationTimer = .0f;
#endif
	}

	void Update() {
		if (vibrationTimer > 0.0f) {
			vibrationTimer -= Time.deltaTime;
		}
	}

}
