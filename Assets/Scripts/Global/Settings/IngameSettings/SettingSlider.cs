using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingSlider : MonoBehaviour
{
    [SerializeField] protected Slider slider;
    [SerializeField] protected TMP_Text settingValue;
    [SerializeField] protected TMP_Text settingName;

    private void Awake()
    {
        slider.onValueChanged.AddListener((float _) => OnSliderChange());
        string[] splitSetting = GetName().Split("_");
        string setting = "";
        foreach (string s in splitSetting)
        {
            setting += s.FirstToUpper() + " ";
        }
        settingName.text = setting;
    }
    protected virtual void OnSliderChange() { }

    public virtual string GetName()
    {
        return "";
    }
}
