using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectSwap", menuName = "ScriptableObjects/CardEffects/Swap")]
public class EffectSwap : CardEffectPlus
{
    private void Reset()
    {
        effectSound = SoundTypeEffect.BLINK;
    }
    public override string GetEffectDescription(Unit player)
    {
        return $"swap places with target";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        BattleUnit target = targetCell.OccupyingUnit();
        if (target.immovable) yield break;
        BattleTileController myCell = actor.OccupiedTile();
        MapTools.ReportPositionSwap(actor, targetCell, target);
        actor.gameObject.transform.position = targetCell.unitPosition;
        target.gameObject.transform.position = myCell.unitPosition;
    }
}
