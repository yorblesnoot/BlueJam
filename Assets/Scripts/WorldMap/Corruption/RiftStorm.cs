using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiftStorm : CorruptionElement
{
    [SerializeField] GameObject stormFX;
    [SerializeField] RunData runData;
    [SerializeField] UnitStateFeedback stateFeedback;

    int budget;
    public override void Activate(int budget)
    {
        this.budget = budget;
        stormFX.SetActive(true);
        EventManager.playerAtWorldLocation.AddListener(DamagePlayerOnStep);
    }

    void DamagePlayerOnStep(Vector2Int _)
    {
        runData.currentHealth -= budget;
        EventManager.updateWorldHealth.Invoke();
        StartCoroutine(stateFeedback.DamageFlash());
        stateFeedback.QueuePopup(budget, Color.red);
    }
}
