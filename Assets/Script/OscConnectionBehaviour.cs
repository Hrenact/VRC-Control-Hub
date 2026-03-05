using UnityEngine;

/// <summary>
/// OSC 连接管理器（单例）
/// 负责：
/// 1. 程序启动时自动连接
/// 2. 支持运行时重新配置
/// </summary>
public class OscConnectionBehaviour : MonoBehaviour
{
    public static OscConnectionBehaviour Instance;

    private OscClient _client = new OscClient();

    private string _ip;
    private int _port;

    private const string KEY = "OSC_SETTINGS";

    [System.Serializable]
    private class OscSettingsData
    {
        public string ip = "127.0.0.1";
        public int port = 9000;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        LoadAndConfigure();
    }

    /// <summary>
    /// 启动时读取保存的配置
    /// </summary>
    public void LoadAndConfigure()
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

        Configure(data.ip, data.port);
    }

    /// <summary>
    /// 配置 OSC 连接（支持运行时重连）
    /// </summary>
    public void Configure(string ip, int port)
    {
        _ip = ip;
        _port = port;

        // 如果你的 OscClient 支持 Close，可以在这里先关闭旧连接
        // _client.Close();

        _client.Configure(ip, port);

        Debug.Log($"OSC 已连接 → {ip}:{port}");
    }

    /// <summary>
    /// 发送 OSC 消息
    /// </summary>
    public void Send(string address, System.Action<OscPacketBuilder> buildAction)
    {
        var builder = new OscPacketBuilder();
        builder.SetAddress(address);

        buildAction?.Invoke(builder);

        _client.Send(builder.Build());
    }
}