using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

/// <summary>
/// OSC 设置 UI 管理
/// 负责：
/// 1. 读取 PlayerPrefs
/// 2. 保存 PlayerPrefs
/// 3. 保存后通知连接管理器立即重连
/// </summary>
public class OscSettingsBehaviour : MonoBehaviour
{
    [Header("UI 输入框")]
    public TMP_InputField ipField;
    public TMP_InputField portField;
    public Toggle safeModeToggle;
    public Toggle chatboxToggle;
    public Toggle controlToggle;
    public Toggle safeareaToggle;
    public Toggle vibrationToggle;
    public TMP_Dropdown dropdownLanguage;

    private const string KEY = "OSC_SETTINGS";

    [System.Serializable]
    public class OscSettingsData
    {
        public string ip = "127.0.0.1";
        public int port = 9000;
        public bool safeMode = true;
        public bool chatboxTip = true;
        public bool controlTip = true;
        public bool safearea = true;
        public bool vibration = true;
        public int language = 0;
    }

    public static OscSettingsBehaviour Instance;

    void Start()
    {
        Instance = this;

        LoadSettings();
        Application.targetFrameRate = 120; // 设置目标帧率
    }

    void LoadSettings()
    {
        OscSettingsData data;

        if (!PlayerPrefs.HasKey(KEY))
        {
            data = new OscSettingsData();
        }
        else
        {
            string json = PlayerPrefs.GetString(KEY);
            data = JsonUtility.FromJson<OscSettingsData>(json);
        }

        ipField.text = data.ip;
        portField.text = data.port.ToString();
        safeModeToggle.isOn = data.safeMode;
        chatboxToggle.isOn = data.chatboxTip;
        controlToggle.isOn = data.controlTip;
        safeareaToggle.isOn = data.safearea;
        vibrationToggle.isOn = data.vibration;
        dropdownLanguage.value = data.language;

        SetLanguage(dropdownLanguage.value);
    }

    /// <summary>
    /// 点击保存按钮时调用
    /// </summary>
    public void Save()
    {
        string ip = ipField.text;

        int port;
        if (!int.TryParse(portField.text, out port))
        {
            Debug.LogWarning("端口格式错误，已使用默认端口 9000");
            port = 9000;
        }

        OscSettingsData data = new OscSettingsData
        {
            ip = ip,
            port = port,
            safeMode = safeModeToggle.isOn,
            chatboxTip = chatboxToggle.isOn,
            controlTip = controlToggle.isOn,
            safearea = safeareaToggle.isOn,
            vibration = vibrationToggle.isOn,
            language = dropdownLanguage.value
        };

        string json = JsonUtility.ToJson(data);

        // 写入 PlayerPrefs
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();

        Debug.Log("OSC 设置已保存：" + json);

        // ✅ 关键：保存后立即重连
        if (OscConnectionBehaviour.Instance != null)
        {
            OscConnectionBehaviour.Instance.Configure(ip, port);
        }
        else
        {
            Debug.LogWarning("OscConnectionBehaviour 尚未初始化");
        }

        SetLanguage(dropdownLanguage.value);
    }

    // 语言切换

    public void SetLanguage(int languageIndex)
    {
        foreach (var text in FindObjectsOfType<LocalizedText>())
        {
            text.Refresh();
        }
    }
}