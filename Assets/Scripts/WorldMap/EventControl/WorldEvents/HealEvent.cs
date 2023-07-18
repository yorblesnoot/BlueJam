using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEvent : WorldEvent
{
    readonly int healFactor = 20;
    readonly int maxFactor = 5;
    public override void Activate()
    {
        runData.playerStats.maxHealth += maxFactor;
        runData.currentHealth += healFactor;
        runData.currentHealth = Mathf.Clamp(runData.currentHealth, 0, runData.playerStats.maxHealth);
        EventManager.updateWorldHealth.Invoke();
        base.Activate();
    }
}
