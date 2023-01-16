using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [HideInInspector] public bool IsOn {get {return isOn;}}
    protected bool isOn;
    protected bool onStart;

    public virtual void Init()
    {
        //isOn = true;
        onStart = true;
    }

    public virtual void OnStart()
    {
        isOn = true;
    }

    public virtual void Stop()
    {
        isOn = false;
    }

    public void OnWin()
    {
        Stop();
    }
}
