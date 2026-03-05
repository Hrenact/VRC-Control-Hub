using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonVibration : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Button targetButton;

    public void OnPointerDown(PointerEventData eventData)
    {
        Vibration.ButtonPress();
    }

    // 松开
    public void OnPointerUp(PointerEventData eventData)
    {
        Vibration.ButtonRelease();
    }
}