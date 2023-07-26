using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsImporter : MonoBehaviour
{
    [SerializeField] DevSettings devProfile;
    [SerializeField] PlayerSettings playerProfile;
    private void Awake()
    {
        if(Settings.Dev == null) Settings.Dev = devProfile;
    }
}
