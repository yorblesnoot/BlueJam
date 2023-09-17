using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionHealth", menuName = "ScriptableObjects/CardEffects/Conditional/Health")]
public class ConditionHealth : EffectConditional
{
    public override string GetEffectDescription(Unit unit)
    {
        return $"if {evaluated}'s health is {comparison} {scalingMultiplier}%, {conditionedEffect.GetEffectDescription(unit)}";
    }
    internal override bool ConditionIsMet(BattleUnit actor, BattleTileController targetCell)
    {
        BattleUnit target = actor;
        if (evaluated == ConditionRecipient.target) target = targetCell.unitContents;
        float percent = target.currentHealth / target.loadedStats[StatType.MAXHEALTH] * 100;
        if (comparison == ConditionDirection.above && percent >= scalingMultiplier) return true;
        if (comparison == ConditionDirection.below && percent <= scalingMultiplier) return true;
        return false;
    }
}
