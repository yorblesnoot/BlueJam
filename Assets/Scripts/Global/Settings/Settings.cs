using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SoundSetting
{
    master_volume,
    music_volume,
    fx_volume,
}

public enum GraphicSetting
{
    ResolutionHeight,
    ResolutionWidth,
    Fullscreen,
    VSync
}
public static class Settings
{
    public static BalanceSettings Balance;
    public static AdminSettings Admin;
    public static GameplaySettings Gameplay;

    public static Dictionary<SoundSetting, float> Sound;

    public static void LoadPlayerSettings()
    {
        Gameplay.Initialize();
        Sound = new();
        foreach (SoundSetting value in Enum.GetValues(typeof(SoundSetting)))
        {
            Sound.Add(value, PlayerPrefs.GetFloat(value.ToString(), 1f));
        }
    }

    public static void UpdateSetting(SoundSetting setting, float value)
    {
        PlayerPrefs.SetFloat(setting.ToString(), value);
        Sound[setting] = value;
    }

    public static void UpdateSetting(GameplaySetting setting, float value)
    {
        PlayerPrefs.SetFloat(setting.ToString(), value);
        Gameplay[setting] = value;
    }

    public static class Graphics
    {
        public static void ImplementSettings()
        {
#if UNITY_ANDROID
            return;
#endif
            bool fs = PlayerPrefs.GetInt(GraphicSetting.Fullscreen.ToString()) == 1;
            FullScreenMode mode = fs ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            Screen.fullScreenMode = mode;
            Resolution maxRes = Screen.resolutions.Last();
            Screen.SetResolution(PlayerPrefs.GetInt(GraphicSetting.ResolutionWidth.ToString(), maxRes.width),
                PlayerPrefs.GetInt(GraphicSetting.ResolutionHeight.ToString(), maxRes.height), fs);
            Screen.fullScreen = fs;
            QualitySettings.vSyncCount = PlayerPrefs.GetInt(GraphicSetting.VSync.ToString());
        }
    }
}
