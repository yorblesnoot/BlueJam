using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public enum BalanceParameter
{
    BossDistance,
    StepsPerThreat,
    ThreatHandicap,
    BaseEncounterSize,
    ThreatPerEncounterSizeUp,
    EnemyStatsPerThreat,
    EnemyHealthPerThreat,
    EnemySpeedPerThreat,
    MinimumDeckSize,
    HesitationCurses
}

[CreateAssetMenu(fileName = "BalanceSettings", menuName = "ScriptableObjects/BalanceSettings")]
public class BalanceSettings : ScriptableObject
{
    public float this[BalanceParameter i]
    {
        get { return loadedParameters[i]; }
    }

    [field: SerializeField] public int HesitationCurses { get; private set; }

    public Dictionary<BalanceParameter, float> loadedParameters;
    [SerializeField] List<SerializedParameter> parameters;

    [System.Serializable]
    class SerializedParameter
    {
        public BalanceParameter Parameter;
        public float Value;
    }

    void LoadParameters()
    {
        loadedParameters = new();
        foreach(var parameter in parameters)
        {
            loadedParameters.Add(parameter.Parameter, parameter.Value);
        }
    }

    public void CombineDifficulties(List<BalanceSettings> settings)
    {
        foreach(var item in settings)
        {
            item.LoadParameters();
            foreach(var key in item.loadedParameters.Keys)
            {
                if (loadedParameters.TryGetValue(key, out float val))
                {
                    loadedParameters[key] += val;
                }
                else
                {
                    loadedParameters.Add(key, val);
                }
            }
            
        }
    }

    public string GetDifferenceDescription()
    {
        string output = "";
        foreach (var parameter in parameters)
        {
            float difference = parameter.Value;
            output += $"{parameter.Parameter.ToString().SplitCamelCase()}" +
            (difference > 0 ? "<color=green>+</color>" : "<color=red>-</color>") +
            Environment.NewLine;
        }
        return output;
    }
}
