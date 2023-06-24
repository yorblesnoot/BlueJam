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

    public override string GenerateDescription(IPlayerData player)
    {
        string triggerName = triggeringEffect.GetType().Name;
        triggerName.Replace("Effect", "");
        string description = $"for {duration} actions, {triggeredEffect.GenerateDescription(player)} on {triggerName}";
        //generate a description somehow..?
        return description;
    }

    public override List<BattleUnit> Execute(BattleUnit actor, BattleTileController targetCell, string[,] aoe)
    {
        List<BattleUnit> toBuff = base.Execute(actor, targetCell, aoe);
        for (int i = 0; i < toBuff.Count; i++)
        {
            toBuff[i].gameObject.GetComponent<TriggerTracker>().RegisterTrigger(this);
        }
        return null;
    }
}
