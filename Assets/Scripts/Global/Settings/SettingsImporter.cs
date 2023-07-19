using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsImporter : MonoBehaviour
{
    [SerializeField] SettingsProfile profile;
    private void Awake()
    {
        if(Settings.Profile == null) Settings.Profile = profile;
    }
}
