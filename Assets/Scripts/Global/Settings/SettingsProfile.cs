using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsProfile", menuName = "ScriptableObjects/SettingsProfile")]
public class SettingsProfile : ScriptableObject
{
    [field: SerializeField] public int BossSpawnDistance { get; private set; }
    [field: SerializeField] public int StepsPerThreat { get; private set; }
    [field: SerializeField] public int InitialThreatPenalty { get; private set; }
    private void Reset()
    {
        BossSpawnDistance = 50;
        StepsPerThreat = 10;
        InitialThreatPenalty = 2;
    }
}
