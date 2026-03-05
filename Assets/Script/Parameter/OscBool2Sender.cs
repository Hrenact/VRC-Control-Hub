using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OscBool2Sender : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("OSC Address")]
    public string address1 = "/input/Jump";
    public string address2 = "/input/Jump2";

    [Header("Send Settings")]
    public float sendRate = 30f;           // 每秒發送次數 (Hz)
    public bool sendWhenHeld = true;       // 按住時是否持續發送 true

    [Header("UI Icon")]
    public Image targetIcon;
    public Sprite iconAddress;
    public Sprite iconAddress2;

    [Header("Optional Button Reference (Auto Bind)")]
    public Button targetButton;

    private bool _isPressed = false;
    private float _nextSendTime;
    private bool _addressSwitched = true; // 用於切換地址的狀態

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
        _addressSwitched = !_addressSwitched; // 切換地址状态
        targetIcon.sprite = _addressSwitched ? iconAddress : iconAddress2;

        _isPressed = true;
        _nextSendTime = Time.time; // 立刻送第一次

        Vibration.ButtonPress();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        _nextSendTime = Time.time; // 立刻送第一次 false

        Vibration.ButtonRelease();
    }

    private void SendBool(bool value)
    {
        string address = _addressSwitched ? address1 : address2;

        OscConnectionBehaviour.Instance.Send(address, builder =>
        {
            builder.AddBool(value);
        });
    }
}