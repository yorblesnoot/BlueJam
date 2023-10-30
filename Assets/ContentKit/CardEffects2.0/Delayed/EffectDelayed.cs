using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EffectDelayed", menuName = "ScriptableObjects/CardEffects/Delayed")]
public class EffectDelayed : CardEffectPlus
{
    public int delay;

    public CardEffectPlus[] delayedEffects;

    public override void Initialize()
    {
        aoe = CellTargeting.CombineAOEIndicators(delayedEffects.Select(x => { x.Initialize(); return x.aoe; }).ToList());
    }

    public override string GetEffectDescription(Unit player)
    {
        //string lapseDescription = delayedEffect.GenerateDescription(player);
        return $"{delayedEffects.Select(x => { x.Initialize(); return x.GenerateDescription(player); }).ToList().GenerateOxfordList()}" +
            $" on the target tile after player spends <color=blue>{delay}</color> pips";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        FutureEffectDispenser.SpawnDelayedEffect(actor, targetCell, delayedEffects, delay);
        yield break;
    }
}

