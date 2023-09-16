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
    public static Dictionary<SoundSetting, float> Player;

    public static void LoadPlayerSettings()
    {
        Player = new();
        foreach (SoundSetting value in Enum.GetValues(typeof(SoundSetting)))
        {
            Player.Add(value, PlayerPrefs.GetFloat(value.ToString(), 1f));
        }
    }

    public static void UpdateSetting(SoundSetting setting, float value)
    {
        Player[setting] = value;
    }

    public static class Graphics
    {
        public static void ImplementSettings()
        {
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
