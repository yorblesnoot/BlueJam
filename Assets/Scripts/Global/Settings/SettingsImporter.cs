using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsImporter : MonoBehaviour
{
    [SerializeField] DevSettings devProfile;
    private void Awake()
    {
        if(Settings.Dev == null) Settings.Dev = devProfile;
        Settings.LoadPlayerSettings();
    }
}
