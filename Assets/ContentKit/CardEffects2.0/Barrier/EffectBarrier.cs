using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectBarrier", menuName = "ScriptableObjects/CardEffects/Barrier")]
public class EffectBarrier : CardEffectPlus
{
    enum BarrierType { DEFLECT, SHIELD }
    [SerializeField] BarrierType barrierType;

    public override string GenerateDescription(IPlayerData player)
    {
        string description = "";
        string subDescription = $"<color=#1ED5FA>{player.barrierScaling * scalingMultiplier}</color>";
        if (barrierType == BarrierType.DEFLECT)
            description = "deflect for [barrier]";
        else if (barrierType == BarrierType.SHIELD)
            description = "shield for [barrier]";
        return description;
    }
    public override List<BattleUnit> Execute(BattleUnit actor, BattleTileController targetCell, string[,] aoe)
    {
        List < BattleUnit > targets = base.Execute(actor, targetCell, aoe);
        foreach (BattleUnit targetUnit in targets)
        {
            BarrierTracker barrierTracker = targetUnit.gameObject.GetComponent<BarrierTracker>();
            int barrier = Mathf.RoundToInt(targetUnit.barrierScaling * scalingMultiplier);
            if (barrierType == BarrierType.DEFLECT) barrierTracker.AddDeflect(barrier);
            else if (barrierType == BarrierType.SHIELD) barrierTracker.AddShield(barrier);
        }
        return null;
    }
}
