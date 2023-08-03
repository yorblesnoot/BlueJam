using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModStat", menuName = "ScriptableObjects/ItemMods/Stat")]
public class ModStat : ItemMod
{
    [SerializeField] StatType statType;
    public override void ModifyPlayer(RunData player)
    {
        Stat(scaler, player);
    }

    void Stat(float scale, RunData runData)
    {
        UnitStats player = runData.playerStats;
        float scaleMult = 1 + scale / 100;
        switch (statType)
        {
            case (StatType.HANDSIZE):
                player.handSize += Mathf.RoundToInt(scale);
                break;
            case (StatType.MAXHEALTH):
                player.maxHealth = Mathf.RoundToInt(player.maxHealth * scaleMult);
                runData.currentHealth = Mathf.Clamp(runData.currentHealth, 0, player.maxHealth);
                EventManager.updateWorldHealth.Invoke();
                break;
            case (StatType.SPEED):
                player.turnSpeed *= scaleMult;
                break;
            case (StatType.DAMAGE):
                player.damageScaling = Mathf.RoundToInt(player.damageScaling * scaleMult);
                break;
            case (StatType.BARRIER):
                player.barrierScaling = Mathf.RoundToInt(player.barrierScaling * scaleMult);
                break;
            case (StatType.HEAL):
                player.healScaling = Mathf.RoundToInt(player.healScaling * scaleMult);
                break;
        }
    }
}