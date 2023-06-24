using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectBlink", menuName = "ScriptableObjects/CardEffects/Blink")]
public class EffectBlink : CardEffectPlus
{
    public override string GenerateDescription()
    {
        description = $"Blink to the target cell";
        return description;
    }
    public override List<BattleUnit> Execute(BattleUnit actor, BattleTileController targetCell, string[,] aoe)
    {
        base.Execute(actor, targetCell, aoe);
        GridTools.ReportPositionChange(actor, targetCell);
        Vector3 destination = targetCell.unitPosition;
        actor.transform.position = destination;
        return null;
    }
}
