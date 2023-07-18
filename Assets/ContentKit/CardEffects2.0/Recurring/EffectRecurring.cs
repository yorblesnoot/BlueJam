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

    public override string GetEffectDescription(IPlayerStats player)
    {
        turnLapseEffect.Initialize();
        string description = "";
        string lapseDescription = turnLapseEffect.GetEffectDescription(player);
        lapseDescription = turnLapseEffect.AppendAOEInfo(lapseDescription);
        if (effectClass == CardClass.ATTACK)
            description = $"debuff target: {lapseDescription} for {duration} actions";
        else if (effectClass == CardClass.BUFF)
            description = $"buff target: {lapseDescription} for {duration} actions";
        return description;
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach(BattleUnit target in targets) target.gameObject.GetComponent<BuffTracker>().RegisterRecurring(actor, this);
        yield return null;
    }
}
