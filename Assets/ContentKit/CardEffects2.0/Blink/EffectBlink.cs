using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectBlink", menuName = "ScriptableObjects/CardEffects/Blink")]
public class EffectBlink : CardEffectPlus
{
    public override string GenerateDescription(IPlayerData player)
    {
        return $"blink to target";
    }
    public override void ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        base.Execute(actor, targetCell);
        MapTools.ReportPositionChange(actor, targetCell);
        Vector3 destination = targetCell.unitPosition;
        actor.transform.position = destination;
    }
}
