using UnityEngine;
using TMPro;
using System.Text;

public class OscChatboxSender : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputField;

    [Header("Counter UI")]
    public TMP_Text byteCounterText;
    public TMP_Text lineCounterText;

    [Header("Settings")]
    public bool sendImmediately = true;
    public bool playNotificationSFX = true;

    private const int MAX_BYTES = 144;
    private const int MAX_LINES = 9;

    void Start()
    {
        if (inputField != null)
            inputField.onValueChanged.AddListener(OnTextChanged);

        UpdateCounters("");
    }

    void OnTextChanged(string text)
    {
        UpdateCounters(text);
    }

    void UpdateCounters(string text)
    {
        // ===== 计算 UTF8 字节数 =====
        int byteCount = Encoding.UTF8.GetByteCount(text);
        // int remainingBytes = MAX_BYTES - byteCount;

        if (byteCounterText != null)
        {
            byteCounterText.text = $"{byteCount} / {MAX_BYTES}";
            byteCounterText.color = byteCount > MAX_BYTES ? Color.red : Color.white;
        }

        // ===== 计算行数 =====
        int lineCount = 0;

        if (!string.IsNullOrEmpty(text))
            lineCount = text.Split('\n').Length;

        if (lineCounterText != null)
        {
            lineCounterText.text = $"{lineCount} / {MAX_LINES}";
            lineCounterText.color = lineCount > MAX_LINES ? Color.red : Color.white;
        }
    }

    public void SendInputFieldText()
    {
        if (inputField == null)
            return;

        SendText(inputField.text);
    }

    public void SendText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        // ===== 按字节裁剪 =====
        byte[] bytes = Encoding.UTF8.GetBytes(text);

        if (bytes.Length > MAX_BYTES)
        {
            text = Encoding.UTF8.GetString(bytes, 0, MAX_BYTES);
        }

        OscConnectionBehaviour.Instance.Send("/chatbox/input", builder =>
        {
            builder.AddString(text);
            builder.AddBool(sendImmediately);
            builder.AddBool(playNotificationSFX);
        });

        Debug.Log($"Chatbox Sent → \"{text}\"");
    }

    public void SetTyping(bool state)
    {
        OscConnectionBehaviour.Instance.Send("/chatbox/typing", builder =>
        {
            builder.AddBool(state);
        });
    }
}