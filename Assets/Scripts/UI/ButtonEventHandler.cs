using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent OnEnterMouse, OnExitMouse;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnterMouse.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExitMouse.Invoke();
    }
}
