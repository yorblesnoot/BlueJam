using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsProfile", menuName = "ScriptableObjects/SettingsProfile")]
public class SettingsProfile : ScriptableObject
{
    [field: SerializeField] public int BossSpawnDistance { get; private set; }
    [field: SerializeField] public int StepsPerThreat { get; private set; }
    [field: SerializeField] public int ThreatHandicap { get; private set; }
    [field: SerializeField] public int BaseFoeBudget { get; private set; }
    [field: SerializeField] public int ThreatPerBudgetUp { get; private set; }

    private void Reset()
    {
        BossSpawnDistance = 50;
        StepsPerThreat = 10;
        ThreatHandicap = 2;
        BaseFoeBudget = 4;
        ThreatPerBudgetUp = 2;
    }
}
