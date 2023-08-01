using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdminSettings", menuName = "ScriptableObjects/AdminSettings")]
public class AdminSettings : ScriptableObject
{
    [field: SerializeField] public float FXVolumeMod { get; private set; }
    [field: SerializeField] public float MusicVolumeMod { get; private set; }

    private void Reset()
    {
        FXVolumeMod = .25f;
        MusicVolumeMod = 1f;
    }
}
