using UnityEngine;
using UnityEngine.UI;

public class VSyncControl : MonoBehaviour
{
    [SerializeField] Toggle vSync;
    private void Awake()
    {
        vSync.isOn = PlayerPrefs.GetInt(GraphicSetting.VSync.ToString()) == 1;
    }

    public void ToggleVSync()
    {
        SoundManager.PlaySound(SoundType.BUTTONPRESS);
        PlayerPrefs.SetInt(GraphicSetting.VSync.ToString(), vSync.isOn ? 1 : 0);
        Settings.Graphics.ImplementSettings();
    }
}
