using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameplaySetting
{
    NPC_card_highlight_duration,
}

[CreateAssetMenu(fileName = "GameplaySettings", menuName = "ScriptableObjects/GameplaySettings")]
public class GameplaySettings : ScriptableObject
{
    [System.Serializable]
    class SettingRange
    {
        public GameplaySetting setting;
        
        public float min;
        public float max;
        public float average { get { return (max-min)/2; } }
        public string qualifier;
        public float value;
        
    }

    [SerializeField] SettingRange[] settingRanges;

    Dictionary<GameplaySetting, float> minimums;
    Dictionary<GameplaySetting, float> maximums;
    Dictionary<GameplaySetting, string> qualifiers;

    Dictionary<GameplaySetting, float> values;

    public float this[GameplaySetting setting]
    {
        get { return values[setting]; }
        set { values[setting] = value; }
    }

    public void Initialize()
    {
        minimums = new();
        maximums = new();
        values = new();
        qualifiers = new();
        foreach (var settingRange in settingRanges)
        {
            minimums.Add(settingRange.setting, settingRange.min);
            maximums.Add(settingRange.setting, settingRange.max);
            qualifiers.Add(settingRange.setting, settingRange.qualifier);
            values.Add(settingRange.setting, PlayerPrefs.GetFloat(settingRange.setting.ToString(), settingRange.average));
        }
    }

    public float GetMin(GameplaySetting setting)
    {
        return minimums[setting];
    }

    public float GetMax(GameplaySetting setting)
    {
        return maximums[setting];
    }

    public string GetQualifier(GameplaySetting setting)
    {
        return qualifiers[setting];
    }
}
