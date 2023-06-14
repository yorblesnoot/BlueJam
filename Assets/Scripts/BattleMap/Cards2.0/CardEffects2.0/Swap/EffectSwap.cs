using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectSwap", menuName = "ScriptableObjects/CardEffects/Swap")]
public class EffectMove : CardEffectPlus
{
    public override string GenerateDescription()
    {
        description = $"Swap places with the unit at the target cell";
        return description;
    }
    public override List<BattleUnit> Execute(BattleUnit actor, BattleTileController targetCell, string[,] aoe)
    {
        base.Execute(actor, targetCell, aoe);
        BattleUnit target = targetCell.unitContents;
        BattleTileController myCell = GridTools.VectorToTile(actor.transform.position).GetComponent<BattleTileController>();
        actor.gameObject.transform.position = targetCell.unitPosition;
        target.gameObject.transform.position = myCell.unitPosition;
        GridTools.ReportPositionSwap(actor, targetCell, target);
        return null;
    }
}
