using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class UIJoystick : 
    MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float radius = 100f;

    public Vector2 Value { get; private set; }  // -1 ~ 1 输出

    private Canvas _canvas;
    private Camera _eventCamera;
    private float _radius;

    void Awake()
    {
        _canvas = GetComponentInParent<Canvas>(true);

        _eventCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : _canvas.worldCamera;

        _radius = radius;
        ResetHandle();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vibration.JoystickPress();

        UpdateHandle(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateHandle(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vibration.ButtonRelease();
        
        ResetHandle();
    }

    private void UpdateHandle(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            _eventCamera,
            out Vector2 localPoint
        );

        Vector2 clamped = Vector2.ClampMagnitude(localPoint, _radius);

        handle.anchoredPosition = clamped;

        // 归一化输出 (-1 ~ 1)
        Value = clamped / _radius;
    }

    private void ResetHandle()
    {
        handle.anchoredPosition = Vector2.zero;
        Value = Vector2.zero;
    }
}