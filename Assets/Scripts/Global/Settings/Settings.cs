using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerSetting
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
    public static Dictionary<PlayerSetting, float> Player;

    public static void LoadPlayerSettings()
    {
        Player = new();
        foreach (PlayerSetting value in Enum.GetValues(typeof(PlayerSetting)))
        {
            Player.Add(value, PlayerPrefs.GetFloat(value.ToString(), 1f));
        }
    }

    public static void UpdateSetting(PlayerSetting setting, float value)
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
            Screen.SetResolution(PlayerPrefs.GetInt(GraphicSetting.ResolutionWidth.ToString()),
                PlayerPrefs.GetInt(GraphicSetting.ResolutionHeight.ToString()), fs);
            QualitySettings.vSyncCount = PlayerPrefs.GetInt(GraphicSetting.VSync.ToString());
        }
    }
}
