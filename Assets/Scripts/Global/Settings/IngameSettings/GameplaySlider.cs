using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySlider : SettingSlider
{
    [SerializeField] GameplaySetting thisSetting;

    public override string GetName()
    {
        return thisSetting.ToString();
    }

    private void OnEnable()
    {
        slider.minValue = Settings.Gameplay.GetMin(thisSetting);
        slider.maxValue = Settings.Gameplay.GetMax(thisSetting);
        slider.value = Settings.Gameplay[thisSetting];
        UpdateText();
    }
    protected override void OnSliderChange()
    {
        PlayerPrefs.SetFloat(thisSetting.ToString(), slider.value);
        Settings.UpdateSetting(thisSetting, slider.value);
        UpdateText();
    }

    private void UpdateText()
    {
        string value = slider.value.ToString();
        settingValue.text = (value.Length > 3 ? value[..3] : value) + " " + Settings.Gameplay.GetQualifier(thisSetting);
    }
}
