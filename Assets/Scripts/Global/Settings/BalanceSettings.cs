using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public enum BalanceParameter
{
    BossDistance,
    CorruptionRate,
    StartingCorruption,
    BaseEncounterSize,
    ThreatPerEncounterSizeUp,
    EnemyStatScaling,
    EnemyHealthScaling,
    EnemySpeedScaling,
    MinimumDeckSize,
    HesitationCurses
}

[CreateAssetMenu(fileName = "BalanceSettings", menuName = "ScriptableObjects/BalanceSettings")]
public class BalanceSettings : ScriptableObject
{
    public float this[BalanceParameter i]
    {
        get { 
            if(loadedParameters == null)
            {
                Debug.LogWarning("Defaulted to Easy Difficulty.");
                baseSettings.LoadParameters();
                loadedParameters = baseSettings.loadedParameters;
            }
            return loadedParameters[i];
        }
    }

    [field: SerializeField] public int HesitationCurses { get; private set; }

    public Dictionary<BalanceParameter, float> loadedParameters;
    [SerializeField] List<SerializedParameter> parameters;
    [SerializeField] BalanceSettings baseSettings;

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
        LoadParameters();
        foreach(var item in settings)
        {
            item.LoadParameters();
            foreach(var key in item.loadedParameters.Keys)
            {
                if (loadedParameters.ContainsKey(key))
                {
                    loadedParameters[key] += item.loadedParameters[key];
                }
                else
                {
                    loadedParameters.Add(key, item.loadedParameters[key]);
                }
            }
            
        }
        foreach (var key in loadedParameters.Keys)
        {
            Debug.Log(key + ": " + loadedParameters[key]);
        }
    }

    public string GetDifferenceDescription()
    {
        string output = "<b>Changes:</b>";
        foreach (var parameter in parameters)
        {
            float difference = parameter.Value;
            output += Environment.NewLine + "<color=red>+</color>" +
            $" {parameter.Parameter.ToString().SplitCamelCase()}";
            
        }
        return output;
    }
}
