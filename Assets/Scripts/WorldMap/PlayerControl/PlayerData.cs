using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour, IPlayerData
{
    public int damageScaling { get; set; }
    public int healScaling { get; set; }
    public int barrierScaling { get; set; }
    public int maxHealth { get; set; }
    public int currentHealth { get; set; }
    public RunData runData;
    void Awake()
    {
        damageScaling = runData.playerStats.damageScaling;
        healScaling = runData.playerStats.healScaling;
        barrierScaling = runData.playerStats.barrierScaling;
        maxHealth = runData.playerStats.maxHealth;
        currentHealth = runData.currentHealth;
    }

}
