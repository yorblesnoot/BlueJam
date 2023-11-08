using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionDeck", menuName = "ScriptableObjects/CardEffects/Conditional/Deck")]
public class ConditionDeckSize : EffectConditional
{
    private void Reset()
    {
        evaluated = ConditionRecipient.user;
    }
    public override string GetEffectDescription(Unit unit)
    {
        return $"if {evaluated}'s deck size is {comparison} {scalingMultiplier}, {conditionedEffect.GetEffectDescription(unit)}";
    }
    internal override bool ConditionIsMet(BattleUnit actor, BattleTileController targetCell)
    {
        int deckSize = actor.myHand.deckRecord.deckContents.Count;
        if (comparison == ConditionDirection.above && deckSize > scalingMultiplier) return true;
        else if(comparison == ConditionDirection.below && deckSize < scalingMultiplier) return true;
        else return false;
    }
}

