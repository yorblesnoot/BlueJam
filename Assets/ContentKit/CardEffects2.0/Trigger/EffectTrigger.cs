using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectTrigger", menuName = "ScriptableObjects/CardEffects/Trigger")]
public class EffectTrigger : CardEffectPlus
{
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

    public TileMapShape aoeShapeTrigger;
    public int aoeSizeTrigger;
    public int aoeGapTrigger;

    public override string GenerateDescription(IPlayerStats player)
    {
        string triggerName = triggeringEffect.GetType().Name;
        triggerName.Replace("Effect", "");
        string description = $"for {duration} actions, {triggeredEffect.GenerateDescription(player)} on {triggerName}";
        //generate a description somehow..?
        return description;
    }

    public override void ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].gameObject.GetComponent<TriggerTracker>().RegisterTrigger(this);
        }
    }
}
