using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EffectDamage;

[CreateAssetMenu(fileName = "EffectBuff", menuName = "ScriptableObjects/CardEffects/Buff")]
public class EffectBuff : CardEffectPlus
{
    public bool purgeable;
    public int duration;

    public Color32 iconColor;

    public CardEffectPlus applicationEffect;
    public CardEffectPlus turnLapseEffect;
    public CardEffectPlus removalEffect;

    public override string GenerateDescription(IPlayerStats player)
    {
        string description = "";
        string lapseDescription = turnLapseEffect.GenerateDescription(player);
        if (effectClass == CardClass.ATTACK)
            description = $"debuff target: {lapseDescription} for {duration} actions";
        else if (effectClass == CardClass.BUFF)
            description = $"buff target: {lapseDescription} for {duration} actions";
        return description;
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach(BattleUnit target in targets) target.gameObject.GetComponent<BuffTracker>().RegisterBuff(actor, this);
        yield return null;
    }
}
