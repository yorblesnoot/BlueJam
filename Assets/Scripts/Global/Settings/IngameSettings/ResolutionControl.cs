using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionControl : MonoBehaviour
{
    [SerializeField] TMP_Dropdown drop;
    [SerializeField] Toggle fsToggle;
    Resolution[] descendingRes;

    private void Awake()
    {
        fsToggle.isOn = PlayerPrefs.GetInt(GraphicSetting.Fullscreen.ToString()) == 1;
        descendingRes = Screen.resolutions.Reverse().ToArray();
        descendingRes = descendingRes.Where(res => res.refreshRate == 60).ToArray();
        
        foreach (var option in descendingRes)
        {
            string dropOutput = $"{option.width} x {option.height}";
            drop.options.Add(new TMP_Dropdown.OptionData(dropOutput));
        }
        drop.value = Array.IndexOf(descendingRes, Screen.currentResolution);

        drop.onValueChanged.AddListener(delegate { SetResolution(); });
        fsToggle.onValueChanged.AddListener(delegate { SetResolution(); });
    }

    void SetResolution()
    {
        Resolution selected = descendingRes[drop.value];
        PlayerPrefs.SetInt(GraphicSetting.ResolutionWidth.ToString(), selected.width);
        PlayerPrefs.SetInt(GraphicSetting.ResolutionHeight.ToString(), selected.height);
        PlayerPrefs.SetInt(GraphicSetting.Fullscreen.ToString(), fsToggle.isOn ? 1 : 0);
        Settings.Graphics.ImplementSettings();
    }
}
