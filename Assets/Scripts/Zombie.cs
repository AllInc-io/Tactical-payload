using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Zombie : MonoBehaviour
{
    [FoldoutGroup("Refs")] public BodyPart head;
    [FoldoutGroup("Refs")] public BodyPart armL;
    [FoldoutGroup("Refs")] public BodyPart armR;
    [FoldoutGroup("Refs")] public BodyPart legL;
    [FoldoutGroup("Refs")] public BodyPart legR;
    [FoldoutGroup("Refs")] public BodyPart torso;
    [FoldoutGroup("Refs")] public SkinnedMeshRenderer[] meshRenderers;


    BodyPart[] destroyableParts;


    private void Awake()
    {
        destroyableParts = new BodyPart[3] { head, armL, armR };
    }


    public void TakeDamage()
    {
        
        if(Random.value < 0.3f)
        {
            //lose a body part
            destroyableParts[Random.Range(0, destroyableParts.Length)].Hit();
        }
    }


    public void Explode()
    {

        foreach(BodyPart bodyPart in GetComponentsInChildren<BodyPart>())
        {
            bodyPart.Hit();
        }

    }

    public void SetDissolveValue(float value)
    {
        foreach(SkinnedMeshRenderer renderer in meshRenderers)
        {
            renderer.material.SetFloat("DissolveValue", value);
        }
    }

    public void ResetVisuals()
    {
        SetDissolveValue(0);
        foreach(BodyPart bodyPart in GetComponentsInChildren<BodyPart>())
        {
            bodyPart.Ressucitate();
        }
        transform.localPosition = Vector3.zero;
    }
}
