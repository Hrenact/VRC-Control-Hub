using System.Collections.Generic;
using UnityEngine;

public class I18nManager : MonoBehaviour
{
    public static I18nManager Instance;

    private Dictionary<string, string> dict;

    private void Awake()
    {
        Instance = this;
        // LoadLanguage();
    }

    public void LoadLanguage()
    {
        if (OscSettingsBehaviour.Instance == null)
            return;

        string lang = "zh";

        switch (OscSettingsBehaviour.Instance.dropdownLanguage.value)
        {
            case 0: lang = "zh"; break;
            case 1: lang = "en"; break;
            case 2: lang = "ja"; break;
        }

        TextAsset json = Resources.Load<TextAsset>("i18n/" + lang);

        if (json == null)
        {
            Debug.LogWarning("Language file not found: " + lang + ".json\nMake sure to place it in Resources/i18n/");
            dict = new Dictionary<string, string>(); // 保证不为nul
            return;
        }

        dict = JsonUtility.FromJson<LocalizationData>(json.text).ToDictionary();
    }

    public string Get(string key)
    {
        if (dict != null && dict.ContainsKey(key))
            return dict[key];

        return key; // 找不到就显示key
    }
}

[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}

[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] items;

    public Dictionary<string, string> ToDictionary()
    {
        Dictionary<string, string> d = new Dictionary<string, string>();

        foreach (var item in items)
        {
            d[item.key] = item.value;
        }

        return d;
    }
}