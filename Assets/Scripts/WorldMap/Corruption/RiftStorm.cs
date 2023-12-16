using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiftStorm : CorruptionElement
{
    [SerializeField] GameObject stormFX;
    [SerializeField] RunData runData;
    [SerializeField] UnitStateFeedback stateFeedback;

    public override void Activate(int budget)
    {
        stormFX.SetActive(true);
        runData.currentHealth -= budget;
        EventManager.updateWorldHealth.Invoke();
        StartCoroutine(stateFeedback.DamageFlash());
        stateFeedback.QueuePopup(budget, Color.red);
    }
}
