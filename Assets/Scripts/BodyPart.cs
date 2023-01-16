using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BodyPart : MonoBehaviour
{
    [FoldoutGroup("Refs")] public List<SkinnedMeshRenderer> meshs;

    [FoldoutGroup("Refs")] public Enemy baseEnemy;
    [FoldoutGroup("Refs")] public ParticleSystem destroyFX;

    public BodyPartType type;
    public bool destructible;
    public bool weakness;


    public void Hit()
    {


        destroyFX.gameObject.SetActive(true);

        foreach (SkinnedMeshRenderer mesh in meshs)
        {
            mesh.enabled = false;
        }

    }

    public void Ressucite()
    {
        destroyFX.gameObject.SetActive(false);

        foreach (SkinnedMeshRenderer mesh in meshs)
        {
            mesh.enabled = true;
        }
    }

}

public enum BodyPartType
{
    Head,
    Torso,
    ArmR,
    ArmL,
    LegR,
    LegL
}

