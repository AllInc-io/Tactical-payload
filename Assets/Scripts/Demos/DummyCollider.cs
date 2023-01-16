using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System;

public class DummyCollider : SerializedMonoBehaviour
{
    public List<CollisionCallback> listCollisions;
    public List<CollisionCallback> listTriggers;

    void Awake()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        foreach(CollisionCallback col in listCollisions)
        {
            if(collision.gameObject.tag == col.tag)
                col.callback.Invoke();
        }
    } 

    void OnTriggerEnter(Collider other)
    {
        foreach(CollisionCallback col in listTriggers)
        {
            if(other.gameObject.tag == col.tag)
                col.callback.Invoke();
               
        }
    } 
}

[Serializable]
public struct CollisionCallback
{
    public string tag;
    public UnityEvent callback;
}
