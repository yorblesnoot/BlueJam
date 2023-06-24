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

    [SerializeField] TileMapShape aoeShapeRecurring;
    [SerializeField] int aoeSizeRecurring;
    [SerializeField] int aoeGapRecurring;

    public override string GenerateDescription(IPlayerData player)
    {
        string description = "";
        string lapseDescription = turnLapseEffect.GenerateDescription(player);
        if (effectClass == CardClass.ATTACK)
            description = $"debuff target: {lapseDescription} for {duration} actions";
        else if (effectClass == CardClass.BUFF)
            description = $"buff target: {lapseDescription} for {duration} actions";
        return description;
    }
    public override List<BattleUnit> Execute(BattleUnit actor, BattleTileController targetCell, string[,] aoe)
    {
        List <BattleUnit> targets = base.Execute(actor, targetCell, aoe);
        string[,] aoeRulesRecurring = MapRulesGenerator.Convert(aoeShapeRecurring, aoeSizeRecurring, aoeGapRecurring);
        foreach(BattleUnit target in targets) target.gameObject.GetComponent<BuffTracker>().RegisterBuff(actor, this, aoeRulesRecurring);
        return targets;
    }
}
