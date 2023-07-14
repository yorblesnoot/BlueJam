using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour, IPlayerStats
{
    public int DamageScaling { get; set; }
    public int healScaling { get; set; }
    public int barrierScaling { get; set; }
    public int maxHealth { get; set; }
    public int currentHealth { get; set; }
    public RunData runData;
    void Awake()
    {
        DamageScaling = runData.playerStats.damageScaling;
        healScaling = runData.playerStats.healScaling;
        barrierScaling = runData.playerStats.barrierScaling;
        maxHealth = runData.playerStats.maxHealth;
        currentHealth = runData.currentHealth;
    }

}
