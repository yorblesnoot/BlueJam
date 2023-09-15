using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingSlider : MonoBehaviour
{
    [SerializeField] SoundSetting thisSetting;
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
        slider.value = Settings.Player[thisSetting];
    }
    public void OnSliderChange()
    {
        PlayerPrefs.SetFloat(thisSetting.ToString(), slider.value);
        Settings.UpdateSetting(thisSetting, slider.value);
        settingValue.text = Mathf.RoundToInt(slider.value * 100).ToString() + "%";
        if (thisSetting == SoundSetting.master_volume || thisSetting == SoundSetting.music_volume || thisSetting == SoundSetting.fx_volume)
        {
            SoundManager.UpdateVolume();
        }
            
    }
}
