using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectConditional : CardEffectPlus
{
    internal enum ConditionRecipient
    {
        user,
        target
    }
    [SerializeField] internal ConditionRecipient evaluated;

    internal enum ConditionDirection
    {
        above,
        below
    }
    [SerializeField] internal ConditionDirection comparison;
    [SerializeField] internal CardEffectPlus conditionedEffect;
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        if(!ConditionIsMet(actor, targetCell)) yield break;
        yield return actor.StartCoroutine(conditionedEffect.ActivateEffect(actor, targetCell, aoe, targets));
    }

    virtual internal bool ConditionIsMet(BattleUnit actor, BattleTileController targetCell)
    {
        return false;
    }
}