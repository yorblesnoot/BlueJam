using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTracker : MonoBehaviour
{
    public List<TrackedTrigger> activeTriggers = new();

    //debugging for triggers
    public EffectTrigger forceTrigger;

    public class TrackedTrigger
    {
        public EffectTrigger trigger;
        public int remainingActivations;
        public bool availableForTrigger;
    }

    private void Awake()
    {
        EventManager.allowTriggers.AddListener(BeginTriggerRound);
        EventManager.checkForTriggers.AddListener(ExecutedEffect);
        //debugging for triggers
        if(forceTrigger != null) activeTriggers.Add(new TrackedTrigger { trigger = forceTrigger, remainingActivations = forceTrigger.triggersRequiredForActivation });
    }
    public void RegisterTrigger(EffectTrigger incomingTrigger)
    {
        activeTriggers.Add(new TrackedTrigger {trigger = incomingTrigger, remainingActivations = incomingTrigger.triggersRequiredForActivation});
    }
    public void ExecutedEffect(CardEffectPlus effect, GameObject origin, GameObject target)
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
                       (tracked.trigger.triggerIdentityCondition == EffectTrigger.TriggerIdentity.RECEIVER && target == gameObject))
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
                                tracked.trigger.triggeredEffect.Execute(gameObject, GridTools.VectorToTile(target.transform.position), new string[,] { { "n" } });
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
