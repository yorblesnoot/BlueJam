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
}
