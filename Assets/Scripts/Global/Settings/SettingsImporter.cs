using UnityEngine;

public class SettingsImporter : MonoBehaviour
{
    [SerializeField] BalanceSettings devProfile;
    [SerializeField] AdminSettings adminSettings;
    [SerializeField] GameplaySettings gameplaySettings;
    private void Awake()
    {
        if (Settings.Balance == null)
        {
            Debug.LogWarning("imported default balance settings");
            Settings.Balance = devProfile;
        }
        if (Settings.Admin == null) Settings.Admin = adminSettings;
        if (Settings.Gameplay == null) Settings.Gameplay = gameplaySettings;
        Settings.LoadPlayerSettings();
        Settings.Graphics.ImplementSettings();
    }
}
