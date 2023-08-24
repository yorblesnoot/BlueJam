using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectStat", menuName = "ScriptableObjects/CardEffects/Stat")]
public class EffectStat : CardEffectPlus
{
    
    [SerializeField] StatType entityStat;
    public int duration;
    private void Reset()
    {
        effectSound = SoundTypeEffect.STATUP;
        effectClass = CardClass.BUFF;
        duration = 0;
    }

    public override string GetEffectDescription(Unit player)
    {
        string finalDescription = $"{(scalingMultiplier > 0 ? "increase" : "decrease")} {statNames[entityStat]}"
            + $" by {Mathf.Abs(scalingMultiplier)}{statModSymbol[entityStat]}";
        if (duration > 0) finalDescription += $" for {duration - 1} {(duration > 2 ? "actions" : "action")}";
        return finalDescription;
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit target in targets)
        {
            Modify(scalingMultiplier, target);
            if (duration > 0) target.GetComponent<BuffTracker>().RegisterTempStat(this);
        }
        yield return null;
    }

    void Modify(float scale, BattleUnit target)
    {
        if(entityStat == StatType.HANDSIZE || entityStat == StatType.BEATS)
            target.loadedStats[entityStat] += scale;
        else target.loadedStats[entityStat] *= 1 + scale / 100;
        if(target.gameObject.CompareTag("Player")) target.gameObject.GetComponent<PlayerHandPlus>().UpdateHand();
        if (entityStat == StatType.BEATS) target.myUI.UpdateBeats(-scale);
    }

    public void Unmodify(float scale, BattleUnit target)
    {
        if (entityStat == StatType.HANDSIZE || entityStat == StatType.BEATS)
            target.loadedStats[entityStat] -= scale;
        else target.loadedStats[entityStat] /= 1 + scale / 100;
        if (target.gameObject.CompareTag("Player")) target.gameObject.GetComponent<PlayerHandPlus>().UpdateHand();
    }

    readonly Dictionary<StatType, string> statNames = new()
    {
        {StatType.MAXHEALTH, "max health" },
        {StatType.HANDSIZE, "hand size"},
        {StatType.SPEED, "speed"},
        {StatType.BEATS, "time"},
        {StatType.DAMAGE, "damage"},
        {StatType.HEAL, "healing" },
        {StatType.BARRIER, "barriers"}
    };
    readonly Dictionary<StatType, string> statModSymbol = new()
    {
        {StatType.MAXHEALTH, "%" },
        {StatType.HANDSIZE, ""},
        {StatType.SPEED, "%"},
        {StatType.BEATS, ""},
        {StatType.DAMAGE, "%"},
        {StatType.HEAL, "%" },
        {StatType.BARRIER, "%"}
    };
}
