using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectRecurring", menuName = "ScriptableObjects/CardEffects/Recurring")]
public class EffectRecurring : CardEffectPlus
{
    public bool purgeable;
    public int duration;

    public Color32 iconColor;

    public CardEffectPlus turnLapseEffect;

    public override string GetEffectDescription(IUnitStats player)
    {
        turnLapseEffect.Initialize();
        string lapseDescription = turnLapseEffect.GenerateDescription(player);
        return $"on target: {lapseDescription} each action for <color=blue>{duration}</color> actions";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach(BattleUnit target in targets) target.gameObject.GetComponent<BuffTracker>().RegisterRecurring(actor, this);
        yield return null;
    }
}
