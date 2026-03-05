using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    public string ChineseText;
    public string EnglishText;
    public string JapaneseText;

    private TMP_Text text;

    // public static LocalizedText Instance;
    // 我不知道为什么当时要用它

    private void Awake()
    {
        //Instance = this;

        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void OnDisable()
    {
        return; // 如果被禁用则不执行任何操作，保持当前文本不变
    }

    public void Refresh()
    {
        if (OscSettingsBehaviour.Instance == null)
        {
            return;
        }

        switch (OscSettingsBehaviour.Instance.dropdownLanguage.value)
        {
            case 0:
                text.text = ChineseText;
                break;
            case 1:
                text.text = EnglishText;
                break;
            case 2:
                text.text = JapaneseText;
                break;
            default:
                text.text = ChineseText; // 默认中文
                break;
        }
    }
}