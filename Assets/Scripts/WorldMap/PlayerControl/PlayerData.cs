using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour, IUnitStats
{
    public Dictionary<StatType, float> loadedStats { get; set; }
    public int currentHealth { get; set; }
    public RunData runData;
    void Awake()
    {
        loadedStats = new()
        {
            { StatType.MAXHEALTH, runData.playerStats.maxHealth },
            { StatType.DAMAGE, runData.playerStats.damageScaling },
            { StatType.HEAL, runData.playerStats.healScaling },
            { StatType.BARRIER, runData.playerStats.barrierScaling },
            { StatType.HANDSIZE, runData.playerStats.handSize },
            { StatType.SPEED, runData.playerStats.turnSpeed },
            { StatType.BEATS, runData.playerStats.startBeats },
        };
        currentHealth = runData.currentHealth;
    }

}
