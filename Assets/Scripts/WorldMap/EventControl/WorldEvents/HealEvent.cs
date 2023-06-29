using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEvent : WorldEvent
{
    public int healFactor = 20;
    public override void Activate()
    {
        runData.currentHealth += healFactor;
        runData.currentHealth = Mathf.Clamp(runData.currentHealth, 0, runData.playerStats.maxHealth);
        EventManager.updateWorldHealth.Invoke();
        base.Activate();
    }
}
