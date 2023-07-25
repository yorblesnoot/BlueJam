using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "EffectStat", menuName = "ScriptableObjects/CardEffects/Stat")]
public class EffectStat : CardEffectPlus
{
    enum StatType { MAXHEALTH, HANDSIZE, SPEED, BEATS, DAMAGE, HEAL, BARRIER }
    [SerializeField] StatType entityStat;
    public int duration;
    private void Reset()
    {
        effectClass = CardClass.BUFF;
        duration = 0;
    }

    public override string GetEffectDescription(IUnitStats player)
    {
        string changeDirection;
        if (scalingMultiplier > 0) changeDirection = "increase";
        else if (scalingMultiplier < 0) changeDirection = "decrease";
        else changeDirection = "(SCALING SET TO 0)";
        string finalDescription = $"{changeDirection} {statNames[entityStat]} by {scalingMultiplier * statVisualMultiplier[entityStat] * 10}{statModSymbol[entityStat]}";
        if (duration > 0) finalDescription += $" for {duration} actions";
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
        switch(entityStat)
        {
            case StatType.MAXHEALTH:
                target.maxHealth = Mathf.RoundToInt(target.maxHealth * (1 + scale / 100));
                break;
            case StatType.DAMAGE:
                target.DamageScaling = Mathf.RoundToInt(target.DamageScaling * (1 + scale / 100));
                break;
            case StatType.HEAL:
                target.healScaling = Mathf.RoundToInt(target.healScaling * (1 + scale / 100));
                break;
            case StatType.BARRIER:
                target.barrierScaling = Mathf.RoundToInt(target.barrierScaling * (1 + scale / 100));
                break;

            case StatType.HANDSIZE:
                target.HandSize += Mathf.RoundToInt(scale);
                break;
            case StatType.SPEED:
                target.turnSpeed *= 1+scale;
                target.turnSpeed = Mathf.Clamp(target.turnSpeed, .2f, 3f);
                break;
            case StatType.BEATS:
                target.currentBeats += Mathf.RoundToInt(scale);
                TurnManager.updateBeatCounts.Invoke();
                break;
        }
        if(target.gameObject.CompareTag("Player")) target.gameObject.GetComponent<PlayerHandPlus>().UpdateHand();
    }

    public void Unmodify(float scale, BattleUnit target)
    {
        switch (entityStat)
        {
            case StatType.MAXHEALTH:
                target.maxHealth = Mathf.RoundToInt(target.maxHealth / (1 + scale / 100));
                break;
            case StatType.DAMAGE:
                target.DamageScaling = Mathf.RoundToInt(target.DamageScaling / (1 + scale / 100));
                break;
            case StatType.HEAL:
                target.healScaling = Mathf.RoundToInt(target.healScaling / (1 + scale / 100));
                break;
            case StatType.BARRIER:
                target.barrierScaling = Mathf.RoundToInt(target.barrierScaling / (1 + scale / 100));
                break;

            case StatType.HANDSIZE:
                target.HandSize -= Mathf.RoundToInt(scale);
                break;
            case StatType.SPEED:
                target.turnSpeed /= 1 + scale/100;
                target.turnSpeed = Mathf.Clamp(target.turnSpeed, .2f, 3f);
                break;
            case StatType.BEATS:
                target.currentBeats -= Mathf.RoundToInt(scale);
                TurnManager.updateBeatCounts.Invoke();
                break;
        }
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
    readonly Dictionary<StatType, float> statVisualMultiplier = new()
    {
        {StatType.MAXHEALTH, 1 },
        {StatType.HANDSIZE, .1f},
        {StatType.SPEED, 10},
        {StatType.BEATS, .1f},
        {StatType.DAMAGE, 1},
        {StatType.HEAL, 1 },
        {StatType.BARRIER, 1}
    };
}
