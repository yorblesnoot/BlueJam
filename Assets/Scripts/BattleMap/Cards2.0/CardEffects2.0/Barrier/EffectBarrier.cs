using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectBarrier", menuName = "ScriptableObjects/CardEffects/Barrier")]
public class EffectBarrier : CardEffectPlus
{
    public enum BarrierType { DEFLECT, SHIELD }
    public BarrierType barrierType;

    public override string GenerateDescription()
    {
        if (barrierType == BarrierType.DEFLECT)
            description = "Deflect for [barrier]";
        else if (barrierType == BarrierType.SHIELD)
            description = "Shield for [barrier]";
        return description;
    }
    public override void Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        base.Execute(actor, targetCell, aoe);
        GameObject targetUnit = targetCell.GetComponent<BattleTileController>().unitContents;
        BarrierTracker barrierTracker = targetUnit.GetComponent<BarrierTracker>();
        UnitActions stats = targetUnit.GetComponent<UnitActions>();
        int barrier = Mathf.RoundToInt(stats.barrierScaling * scalingMultiplier);
        if (barrierType == BarrierType.DEFLECT) barrierTracker.AddDeflect(barrier);
        else if (barrierType == BarrierType.SHIELD) barrierTracker.AddShield(barrier);
    }
}
