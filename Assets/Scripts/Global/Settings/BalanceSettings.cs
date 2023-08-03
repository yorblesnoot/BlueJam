using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BalanceSettings", menuName = "ScriptableObjects/BalanceSettings")]
public class BalanceSettings : ScriptableObject
{
    [field: SerializeField] public int BossSpawnDistance { get; private set; }
    [field: SerializeField] public int StepsPerThreat { get; private set; }
    [field: SerializeField] public int ThreatHandicap { get; private set; }
    [field: SerializeField] public int BaseFoeBudget { get; private set; }
    [field: SerializeField] public int ThreatPerBudgetUp { get; private set; }

    [field: SerializeField] public float StatPerThreat { get; private set; }
    [field: SerializeField] public float HealthPerThreat { get; private set; }
    [field: SerializeField] public float SpeedPerThreat { get; private set; }

    [field: SerializeField] public int MinimumDeckSize { get; private set; }

    [field: SerializeField] public int HesitationCurses { get; private set; }





    private void Reset()
    {
        BossSpawnDistance = 40;
        StepsPerThreat = 10;
        ThreatHandicap = 2;
        BaseFoeBudget = 5;
        ThreatPerBudgetUp = 5;

        StatPerThreat = .1f;
        HealthPerThreat = .04f;
        SpeedPerThreat = .01f;

        MinimumDeckSize = 9;
    }
}
