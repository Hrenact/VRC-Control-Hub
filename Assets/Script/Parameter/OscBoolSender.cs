using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OscBoolSender : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("OSC Address")]
    public string address = "/input/Jump";

    [Header("Send Settings")]
    public float sendRate = 30f;           // 每秒發送次數 (Hz)
    public bool sendWhenHeld = true;       // 按住時是否持續發送 true

    [Header("Optional Button Reference (Auto Bind)")]
    public Button targetButton;

    private bool _isPressed = false;
    private float _nextSendTime;

    void Awake()
    {
        // 如果有拖入 Button，嘗試自動綁定（可選）
        if (targetButton != null)
        {
            var trigger = targetButton.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = targetButton.gameObject.AddComponent<EventTrigger>();
            }
            // 這裡只是確保有 EventTrigger 元件，實際事件由 IPointerHandler 處理
        }
    }

    void Update()
    {
        if (Time.time < _nextSendTime)
            return;

        _nextSendTime = Time.time + 1f / sendRate;

        // 根據目前按住狀態，固定頻率送出對應值
        if (_isPressed)
        {
            if (sendWhenHeld)
            {
                SendBool(true);
            }
        }
        else
        {
            SendBool(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
        _nextSendTime = Time.time; // 立刻送第一次

        Vibration.ButtonPress();
        Debug.Log($"OSC → {address} = true (PointerDown)");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        _nextSendTime = Time.time; // 立刻送第一次 false

        Vibration.ButtonRelease();
        Debug.Log($"OSC → {address} = false (PointerUp)");
    }

    // 可選：如果希望支援滑鼠/觸控離開範圍也視為鬆開
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isPressed)
        {
            _isPressed = false;
            _nextSendTime = Time.time;
            SendBool(false);
            Vibration.ButtonRelease();
            Debug.Log($"OSC → {address} = false (PointerExit)");
        }
    }

    private void SendBool(bool value)
    {
        OscConnectionBehaviour.Instance.Send(address, builder =>
        {
            builder.AddBool(value);
        });
    }
}