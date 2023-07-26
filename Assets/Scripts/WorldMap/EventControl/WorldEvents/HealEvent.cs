using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEvent : WorldEvent
{
    readonly float healFactor = .25f;
    readonly int maxFactor = 5;
    
    public override void Activate()
    {
        SoundManager.PlaySound(SoundType.GOTHEART);
        runData.playerStats.maxHealth += maxFactor;
        runData.currentHealth += Mathf.RoundToInt(healFactor * runData.playerStats.maxHealth);
        runData.currentHealth = Mathf.Clamp(runData.currentHealth, 0, runData.playerStats.maxHealth);
        EventManager.updateWorldHealth.Invoke();
        base.Activate();
    }
}
