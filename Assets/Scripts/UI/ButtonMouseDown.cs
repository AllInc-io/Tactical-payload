using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
 
public class ButtonMouseDown : MonoBehaviour, IPointerDownHandler
{
    //Button button;
    public UnityEvent callback;

    void Awake()
    {
        
    }

    public void OnPointerDown(PointerEventData data)
    {
        callback.Invoke();
    }
}

