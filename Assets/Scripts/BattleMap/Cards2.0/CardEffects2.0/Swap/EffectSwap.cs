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
    public override List<GameObject> Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        base.Execute(actor, targetCell, aoe);
        GameObject target = targetCell.GetComponent<BattleTileController>().unitContents;
        GameObject myCell = GridTools.VectorToTile(actor.transform.position);
        actor.transform.position = targetCell.GetComponent<BattleTileController>().unitPosition;
        target.transform.position = myCell.GetComponent<BattleTileController>().unitPosition;
        GridTools.ReportPositionSwap(actor, targetCell, target);
        return null;
    }
}
