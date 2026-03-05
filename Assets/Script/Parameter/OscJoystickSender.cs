using UnityEngine;

public class OscJoystickSender : MonoBehaviour
{
    [Header("OSC Addresses")]
    public string verticalAddress   = "/input/Vertical";
    public string horizontalAddress = "/input/Horizontal";
    public string runAddress        = "/input/Run";

    [Header("Joystick Input")]
    public RectTransform joystickHandle;   // 拖入搖桿 handle
    public float maxRadius = 100f;

    [Header("Run Settings")]
    public bool enableRun = true;
    public float runThreshold = 0.8f;

    [Header("Send Settings")]
    public float sendRate = 30f; // Hz

    private float _nextSendTime;

    void Update()
    {
        if (Time.time < _nextSendTime)
            return;

        _nextSendTime = Time.time + 1f / sendRate;

        // 每固定間隔都送一次目前值
        Vector2 input = GetNormalizedInput();

        // 軸永遠送現在的值（沒拖動時自然就是 0）
        SendAxis(horizontalAddress, input.x);
        SendAxis(verticalAddress,   input.y);

        // Run 狀態也每幀判斷並送（沒拖動時 magnitude 很小 → false）
        if (enableRun)
        {
            bool runState = input.magnitude > runThreshold;
            SendBool(runAddress, runState);
        }
    }

    private Vector2 GetNormalizedInput()
    {
        Vector2 localPos = joystickHandle.anchoredPosition;
        Vector2 normalized = localPos / maxRadius;
        return Vector2.ClampMagnitude(normalized, 1f);
    }

    private void SendAxis(string address, float value)
    {
        OscConnectionBehaviour.Instance.Send(address, b =>
        {
            b.AddFloat(value);
        });
    }

    private void SendBool(string address, bool value)
    {
        OscConnectionBehaviour.Instance.Send(address, b =>
        {
            b.AddBool(value);
        });
    }
}