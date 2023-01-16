using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class LevelDesignScriptableObject : ScriptableObject
{
    //defines curves and values that will be used throughout the games

    [FoldoutGroup("General settings")] public float damagePerSecondWhenOnFire = 10;
    [FoldoutGroup("General settings")] public float maxZombiesProjectionForceWhenKilled = 20f;
    [FoldoutGroup("General settings")] public int levelUnlockSecondChara = 0;
    [FoldoutGroup("General settings")] public int levelUnlockThirdChara = 3;
    [FoldoutGroup("General settings")] public float charactersScale = 0.75f;
    [FoldoutGroup("General settings")] public int moneyWonPerLevel = 500;
    [FoldoutGroup("General settings")] public int moneyWonPerZombieKilled = 3;


    [FoldoutGroup("Difficulty curve")] public AnimationCurve wavesAmountMultiplierCurve;
    [FoldoutGroup("Difficulty curve")] public float wavesStartMultiplier = 1;
    [FoldoutGroup("Difficulty curve")] public float wavesMaxMultiplier = 10;
    [FoldoutGroup("Difficulty curve")] public int maxLevel = 100;

    [FoldoutGroup("Character")] public Hero[] possibleCharactersPrefabs;
    [FoldoutGroup("Character")] public int newCharacterEveryX;


    [FoldoutGroup("Character")] public int characterMaxLevel;

    [FoldoutGroup("Character")] public AnimationCurve characterUpgradePriceCurve;
    [FoldoutGroup("Character")] public int characterUpgradeBasePrice;
    [FoldoutGroup("Character")] public int characterUpgradeMaxMultiplier;

    [FoldoutGroup("Character")] public AnimationCurve characterSpeedBoostCurve;
    [FoldoutGroup("Character")] public float characterSpeedMaxAdditionalMultiplier;

    [FoldoutGroup("Character")] public AnimationCurve characterDamageBoostCurve;
    [FoldoutGroup("Character")] public float characterDamageMaxAdditionalMultiplier;

    [FoldoutGroup("Character")] public AnimationCurve characterFireRateBoostCurve;
    [FoldoutGroup("Character")] public float characterFireRateMaxAdditionalMultiplier;


    public float EvaluateCharaSpeedMultiplier(int level)
    {
        float t = (float) level / characterMaxLevel;
        float value = 1f + characterSpeedBoostCurve.Evaluate(t) * characterSpeedMaxAdditionalMultiplier;
        return value;
    }

    public float EvaluateCharaDamageMultiplier(int level)
    {
        float t = (float) level / characterMaxLevel;
        float value = 1f + characterDamageBoostCurve.Evaluate(t) * characterDamageMaxAdditionalMultiplier;
        return value;
    }

    public float EvaluateCharaFirerateMultiplier(int level)
    {
        float t = (float) level / characterMaxLevel;
        float value = 1f + characterFireRateBoostCurve.Evaluate(t) * characterFireRateMaxAdditionalMultiplier;
        return value;
    }

    public int EvaluateUpgradePrice(int level)
    {
        float t = (float)level / characterMaxLevel;
        float value = characterUpgradeBasePrice + characterUpgradeBasePrice * characterUpgradePriceCurve.Evaluate(t) * characterUpgradeMaxMultiplier;
        return Mathf.RoundToInt(value);
    }

    public int EvaluateWaveAmount(int level, int baseAmount)
    {
        float t = (float)level / maxLevel;
        float value = wavesStartMultiplier + wavesStartMultiplier * wavesAmountMultiplierCurve.Evaluate(t) * wavesMaxMultiplier ;
        return Mathf.RoundToInt(baseAmount * value);
    }
}
