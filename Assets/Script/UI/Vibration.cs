using UnityEngine;
using System;

public static class Vibration
{
    // 是否有震动器（大部分手机都有）
    public static bool HasVibrator()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        return vibrator.Call<bool>("hasVibrator");
#else
        return false;
#endif
    }

    // 简单震动一次（默认 250ms，最大强度）
    public static void Vibrate()
    {
        Vibrate(250);
    }

    // 指定时长（毫秒）
    public static void Vibrate(long milliseconds)
    {
        Vibrate(milliseconds, -1);  // -1 表示默认强度
    }

    // 指定时长 + 强度（幅度 1~255，-1=默认/最大）
    public static void Vibrate(long milliseconds, int amplitude)
    {
        if (!OscSettingsBehaviour.Instance.vibrationToggle.isOn) // 震动开关控制
            return;

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");

        if (vibrator.Call<bool>("hasVibrator"))
        {
            if (AndroidBuildVersion >= 26)  // API 26+ 支持幅度
            {
                AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                AndroidJavaObject effect;

                if (amplitude == -1)
                    amplitude = vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE");  // 默认 ≈255

                amplitude = Mathf.Clamp(amplitude, 1, 255);

                effect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                vibrator.Call("vibrate", effect);
            }
            else  // 老安卓只能控制时长
            {
                vibrator.Call("vibrate", milliseconds);
            }
        }
#endif
    }

    // 震动模式（自定义波形）——比如心跳：短-短-长
    public static void VibratePattern(long[] pattern, int repeat = -1)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");

        if (AndroidBuildVersion >= 26)
        {
            AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", pattern, repeat);
            vibrator.Call("vibrate", effect);
        }
        else
        {
            vibrator.Call("vibrate", pattern, repeat);
        }
#endif
    }

    // 取消当前震动
    public static void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        vibrator.Call("cancel");
#endif
    }

    // 获取当前 Android API 版本（辅助用）
    private static int AndroidBuildVersion
    {
        get
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return version.GetStatic<int>("SDK_INT");
            }
#else
            return 0;
#endif
        }
    }

    /*
    调用示例（你可以在任何需要震动的地方调用这些方法）

    调用预设：Vibration.LightTap();

    调用自定义：Vibration.Vibrate(500, 200);
    自定义含义：Vibrate(时长ms, 强度1~255);

    下面是一些预设震动模式，你也可以根据需要自定义更多：
    */

    public static void ButtonPress()     => Vibrate(30, 200);     // 按钮按下
    public static void ButtonRelease()   => Vibrate(30, 100);      // 按钮松开
    
    public static void JoystickPress()    => Vibrate(30, 100);     // 摇杆按下
    public static void JoystickRelease()  => Vibrate(30, 200);      // 摇杆松开

    public static void HeartBeat()    // 心跳效果示例
    {
        long[] pattern = { 0, 100, 100, 100, 400 };  // 等待0ms → 震100 → 停100 → 震100 → 停400
        VibratePattern(pattern);
    }
}