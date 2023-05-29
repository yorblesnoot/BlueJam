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

    public override string GenerateDescription()
    {
        string lapseDescription = turnLapseEffect.GenerateDescription();
        if (effectClass == CardClass.ATTACK)
            description = $"Applies a debuff that {lapseDescription} each turn for {duration} turns [target]";
        else if (effectClass == CardClass.BUFF)
            description = $"Applies a buff that {lapseDescription} each turn for {duration} turns [target]";
        return description;
    }
    public override List<GameObject> Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        List <GameObject> targets = base.Execute(actor, targetCell, aoe);
        string[,] aoeRulesRecurring = MapRulesGenerator.Convert(aoeShapeRecurring, aoeSizeRecurring, aoeGapRecurring);
        foreach(GameObject target in targets) target.GetComponent<BuffTracker>().RegisterBuff(actor, this, aoeRulesRecurring);
        return targets;
    }
}
