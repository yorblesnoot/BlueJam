using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingSlider : MonoBehaviour
{
    [SerializeField] PlayerSetting thisSetting;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text settingValue;
    [SerializeField] TMP_Text settingName;


    private void Awake()
    {
        string[] splitSetting = thisSetting.ToString().Split("_");
        string setting = "";
        foreach (string s in splitSetting)
        {
            setting += s.FirstToUpper() + " ";
        }
        settingName.text = setting;
    }

    private void OnEnable()
    {
        slider.value = PlayerPrefs.GetFloat(thisSetting.ToString());
    }
    public void OnSliderChange()
    {
        PlayerPrefs.SetFloat(thisSetting.ToString(), slider.value);
        Settings.UpdateSetting(thisSetting, slider.value);
        settingValue.text = (slider.value * 100).ToString();
        if (thisSetting == PlayerSetting.master_volume || thisSetting == PlayerSetting.music_volume || thisSetting == PlayerSetting.fx_volume)
            SoundManager.UpdateVolume();
    }
}
