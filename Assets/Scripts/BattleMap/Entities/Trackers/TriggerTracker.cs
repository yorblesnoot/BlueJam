using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTracker : MonoBehaviour
{
    public List<TrackedTrigger> activeTriggers = new();

    //debugging for triggers
    //public EffectTrigger forceTrigger;

    [SerializeField] BattleUnit battleUnit;

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
        //if(forceTrigger != null) activeTriggers.Add(new TrackedTrigger { trigger = forceTrigger, remainingActivations = forceTrigger.triggersRequiredForActivation });
    }
    public void RegisterTrigger(EffectTrigger incomingTrigger)
    {
        activeTriggers.Add(new TrackedTrigger {trigger = incomingTrigger,
            remainingActivations = incomingTrigger.triggersRequiredForActivation});
        incomingTrigger.triggeredEffect.Initialize();
    }
    public void ExecutedEffect(CardEffectPlus effect, BattleUnit origin, BattleUnit target)
    {
        //check this: triggers going the wrong way~~~~~~~~~~~~~~~~~~~~~~~~
        if (effect.blockTrigger != true && activeTriggers.Count > 0)
        {
            foreach (TrackedTrigger tracked in activeTriggers)
            {
                //check if the effect matches the trigger condition and that the trigger hasnt happened yet this round
                if (tracked.availableForTrigger == true && effect.GetType() == tracked.trigger.triggeringEffect.GetType())
                {
                    CheckForTrigger(tracked, origin, target);
                }
            }
        }
    }

    void CheckForTrigger(TrackedTrigger tracked, BattleUnit origin, BattleUnit target)
    {
        //confirm receipt or use of triggering effect
        if ((tracked.trigger.triggerIdentityCondition == EffectTrigger.TriggerIdentity.USER && origin == battleUnit) ||
            (tracked.trigger.triggerIdentityCondition == EffectTrigger.TriggerIdentity.RECEIVER && target == battleUnit))
        {
            //count down activations; if we've activated enough times, execute the effect
            tracked.remainingActivations--;
            tracked.availableForTrigger = false;
            if (tracked.remainingActivations > 0) return;

            Debug.Log(gameObject + " triggered " + tracked.trigger.triggeredEffect);
            BattleTileController targetCell = null;
            if (tracked.trigger.effectRecipient == EffectTrigger.TriggerIdentity.USER)
            {
                Debug.Log($"..on {origin}");
                targetCell = MapTools.VectorToTile(origin.transform.position).GetComponent<BattleTileController>();
            }
            else if (tracked.trigger.effectRecipient == EffectTrigger.TriggerIdentity.RECEIVER)
            {
                Debug.Log($"..on {target}");
                targetCell = MapTools.VectorToTile(target.transform.position).GetComponent<BattleTileController>();
            }
            StartCoroutine(tracked.trigger.triggeredEffect.Execute(battleUnit, targetCell));
            tracked.remainingActivations = tracked.trigger.triggersRequiredForActivation;
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
