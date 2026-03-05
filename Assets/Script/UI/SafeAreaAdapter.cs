using UnityEngine;

public class SafeAreaAdapter : MonoBehaviour
{
    public RectTransform panel;
    private Rect lastSafeArea = Rect.zero;
    private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;

    void Start()
    {
        ApplySafeArea();
    }

    void Update()
    {
        if (OscSettingsBehaviour.Instance.safeareaToggle.isOn) // 只有在安全区域适配开启时才检查变化
        {
            if (Screen.orientation != lastOrientation || Screen.safeArea != lastSafeArea) // 仅在方向或安全区域发生变化时才重新应用
            {
                ApplySafeArea();
            }
        }
        else
        {
            ResetSafeArea();
        }
    }

    public void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;   // 当前安全区域（随旋转动态变化）

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;

        lastSafeArea = safeArea;
        lastOrientation = Screen.orientation;

        // Debug.Log($"[SafeAreaAdapter] Applied safe area: {safeArea}, Orientation: {Screen.orientation}");
        // Log 会导致刷屏，注释
    }

    public void ResetSafeArea()
    {
        panel.anchorMin = Vector2.zero;
        panel.anchorMax = Vector2.one;

        lastSafeArea = Rect.zero;
        lastOrientation = ScreenOrientation.AutoRotation;

        // Debug.Log("[SafeAreaAdapter] Reset safe area to full screen.");
        // Log 会导致刷屏，注释
    }
}
