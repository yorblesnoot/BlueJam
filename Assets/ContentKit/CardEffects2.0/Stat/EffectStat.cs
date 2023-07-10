using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectStat", menuName = "ScriptableObjects/CardEffects/Stat")]
public class EffectStat : CardEffectPlus
{
    enum StatType { MAXHEALTH, HANDSIZE, SPEED, BEATS, DAMAGE, HEAL, BARRIER }
    [SerializeField] StatType entityStat;
    private void Reset()
    {
        effectClass = CardClass.BUFF;
    }
    Dictionary<StatType, string> statNames = new()
    {
        {StatType.MAXHEALTH, "max health" },
        {StatType.HANDSIZE, "intelligence"},
        {StatType.SPEED, "speed"},
        {StatType.BEATS, ""},
        {StatType.DAMAGE, "strength"},
        {StatType.HEAL, "wisdom" },
        {StatType.BARRIER, "constitution"}
    };

    public override string GenerateDescription(IPlayerStats player)
    {
        string changeDirection;
        if (scalingMultiplier > 0) changeDirection = "increase";
        else if (scalingMultiplier < 0) changeDirection = "decrease";
        else changeDirection = "(SCALING SET TO 0)";
        return $"{changeDirection} {statNames[entityStat]} by {scalingMultiplier}";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit target in targets) Modify(scalingMultiplier, target);
        yield return null;
    }

    void Modify(float scale, BattleUnit target)
    {
        int modifier = Mathf.RoundToInt(scale);
        switch(entityStat)
        {
            case StatType.MAXHEALTH:
                target.maxHealth += modifier;
                break;
            case StatType.HANDSIZE:
                target.handSize += modifier;
                break;
            case StatType.SPEED:
                target.turnSpeed += scale;
                break;
            case StatType.BEATS:
                target.currentBeats += scale;
                break;
            case StatType.DAMAGE:
                target.damageScaling += modifier;
                break;
            case StatType.HEAL:
                target.healScaling += modifier;
                break;
            case StatType.BARRIER:
                target.barrierScaling += modifier;
                break;
        }
        if(target.gameObject.tag == "Player")
        {
            target.gameObject.GetComponent<Hand>().UpdateHand();
        }
    }
}
