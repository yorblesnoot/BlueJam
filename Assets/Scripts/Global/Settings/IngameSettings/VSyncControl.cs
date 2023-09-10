using UnityEngine;
using UnityEngine.UI;

public class VSyncControl : MonoBehaviour
{
    [SerializeField] Toggle vSync;

    public void ToggleVSync()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        if (vSync.isOn) QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;
    }
}
