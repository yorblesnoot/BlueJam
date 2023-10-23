using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : SettingSlider
{
    [SerializeField] SoundSetting thisSetting;

    public override string GetName()
    {
        return thisSetting.ToString();
    }

    private void OnEnable()
    {
        slider.value = Settings.Sound[thisSetting];
        UpdateText();
    }
    protected override void OnSliderChange()
    {
        Settings.UpdateSetting(thisSetting, slider.value);
        UpdateText();
        SoundManager.UpdateVolume();   
    }

    void UpdateText()
    {
        settingValue.text = Mathf.RoundToInt(slider.value * 100).ToString() + "%";
    }
}
