using System.Collections.Generic;
using UnityEngine;

public enum StatType { MAXHEALTH, HANDSIZE, SPEED, BEATS, DAMAGE, HEAL, BARRIER }
public class Unit : MonoBehaviour
{
    public Dictionary<StatType, float> loadedStats {  get; set; }
    public int currentHealth { get; set; }
    [field: SerializeField] public UnitStats unitStats { get; set; }

    public ResourceTracker resourceTracker = new();

    public RunData runData;

    public void LoadStats()
    {
        loadedStats = new()
        {
            { StatType.MAXHEALTH, unitStats.maxHealth },
            { StatType.DAMAGE, unitStats.damageScaling },
            { StatType.HEAL, unitStats.healScaling },
            { StatType.BARRIER, unitStats.barrierScaling },
            { StatType.HANDSIZE, unitStats.handSize },
            { StatType.SPEED, unitStats.turnSpeed },
            { StatType.BEATS, unitStats.startBeats },
        };
    }
}
