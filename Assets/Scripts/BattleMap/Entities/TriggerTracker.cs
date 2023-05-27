using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTracker : MonoBehaviour
{
    public List<TrackedTrigger> activeTriggers = new();

    public class TrackedTrigger
    {
        public EffectTrigger trigger;
        public int remainingActivations;
        public bool availableForTrigger;
    }

    private void Awake()
    {
        EventManager.allowTriggers.AddListener(BeginTriggerRound);
    }
    public void RegisterTrigger(EffectTrigger incomingTrigger)
    {
        activeTriggers.Add(new TrackedTrigger {trigger = incomingTrigger, remainingActivations = incomingTrigger.triggersRequiredForActivation});
    }
    public void ExecutedEffect(CardEffectPlus effect, GameObject origin, GameObject targetCell)
    {
        if (effect.blockTrigger != true && activeTriggers.Count > 0)
        {
            foreach (TrackedTrigger tracked in activeTriggers)
            {
                //check if the effect matches the trigger condition and that the trigger hasnt happened yet this round
                if (tracked.availableForTrigger == true && effect.GetType() == tracked.trigger.triggeringEffect.GetType())
                {
                    //confirm receipt or use of triggering effect
                    if ((tracked.trigger.triggerIdentityCondition == EffectTrigger.TriggerIdentity.USER && origin == gameObject) ||
                       (tracked.trigger.triggerIdentityCondition == EffectTrigger.TriggerIdentity.RECEIVER && targetCell.GetComponent<BattleTileController>().unitContents == gameObject))
                    {
                        //count down activations; if we've activated enough times, execute the effect
                        tracked.remainingActivations--;
                        tracked.availableForTrigger = false;
                        if (tracked.remainingActivations == 0)
                        {
                            if (tracked.trigger.effectRecipient == EffectTrigger.TriggerIdentity.USER)
                            {
                                tracked.trigger.triggeredEffect.Execute(gameObject, GridTools.VectorToTile(origin.transform.position), new string[,] { { "n" } });
                            }
                            else if (tracked.trigger.effectRecipient == EffectTrigger.TriggerIdentity.RECEIVER)
                            {
                                tracked.trigger.triggeredEffect.Execute(gameObject, targetCell, new string[,] { { "n" } });
                            }
                            tracked.remainingActivations = tracked.trigger.triggersRequiredForActivation;
                        }
                    }
                }
            }
        }
    }

    void BeginTriggerRound()
    {
        for(int i = 0; i < activeTriggers.Count; i++)
        {
            activeTriggers[i].availableForTrigger = true;
        }
    }
}
