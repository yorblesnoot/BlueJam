using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiftStorm : CorruptionElement
{
    [SerializeField] GameObject stormFX;
    [SerializeField] RunData runData;
    [SerializeField] UnitStateFeedback stateFeedback;

    int tutorialCount = 0;

    public override void Activate(int budget)
    {
        Tutorial.Initiate(TutorialFor.WORLDSTORM, TutorialFor.WORLDMOVE);
        Tutorial.EnterStage(TutorialFor.WORLDSTORM, 1, "<color=purple>The end is nigh.</color> Reach the final boss before the storm claims you!");
        tutorialCount++;
        if (tutorialCount > 4) Tutorial.CompleteStage(TutorialFor.WORLDSTORM, 1, true);
        stormFX.SetActive(true);
        runData.currentHealth -= budget;
        EventManager.updateWorldHealth.Invoke();
        StartCoroutine(stateFeedback.DamageFlash());
        stateFeedback.QueuePopup(budget, Color.red);
    }
}
