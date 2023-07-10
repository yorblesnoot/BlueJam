using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectSwap", menuName = "ScriptableObjects/CardEffects/Swap")]
public class EffectMove : CardEffectPlus
{
    public override string GenerateDescription(IPlayerStats player)
    {
        return $"swap places with target";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        BattleUnit target = targetCell.unitContents;
        BattleTileController myCell = MapTools.VectorToTile(actor.transform.position).GetComponent<BattleTileController>();
        actor.gameObject.transform.position = targetCell.unitPosition;
        target.gameObject.transform.position = myCell.unitPosition;
        MapTools.ReportPositionSwap(actor, targetCell, target);
        yield return null;
    }
}
