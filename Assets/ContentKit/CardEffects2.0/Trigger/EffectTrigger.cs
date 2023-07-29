using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectTrigger", menuName = "ScriptableObjects/CardEffects/Trigger")]
public class EffectTrigger : CardEffectPlus
{
    private void Reset()
    {
        effectClass = CardClass.BUFF;
    }
    public enum TriggerIdentity
    {
        USER,
        RECEIVER
    }
    public bool purgeable;
    public int duration;

    public TriggerIdentity triggerIdentityCondition;
    public CardEffectPlus triggeringEffect;
    [Range(1, 10)]public int triggersRequiredForActivation;

    public TriggerIdentity effectRecipient;
    public CardEffectPlus triggeredEffect;

    public override string GetEffectDescription(Unit player)
    {
        string triggerName = triggeringEffect.GetType().Name;
        triggerName.Replace("Effect", "");
        string description = $"for {duration} actions, {triggeredEffect.GetEffectDescription(player)} on {triggerName}";
        return description;
    }

    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].gameObject.GetComponent<TriggerTracker>().RegisterTrigger(this);
        }
        yield return null;
    }
}
