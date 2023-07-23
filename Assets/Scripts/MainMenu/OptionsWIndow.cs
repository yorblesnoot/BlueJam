using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsWIndow : MonoBehaviour
{
    [SerializeField] Toggle vSync;
    private void Start()
    {
        ToggleVSync();
        gameObject.SetActive(false);
    }
    public void ResetTutorials()
    {
        PlayerPrefs.SetInt(nameof(TutorialFor.MAIN), 0);
    }

    public void ToggleVSync()
    {
        if(vSync.isOn) QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
