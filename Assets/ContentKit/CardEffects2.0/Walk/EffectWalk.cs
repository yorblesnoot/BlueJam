using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectWalk", menuName = "ScriptableObjects/CardEffects/Walk")]
public class EffectWalk : CardEffectPlus
{
    private void Reset()
    {
        effectClass = CardClass.MOVE;
    }
    [Range(.1f, 1f)] public float walkDuration;
    public override string GetEffectDescription(IPlayerStats player)
    {
        return "move to target";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        MapTools.ReportPositionChange(actor, targetCell);
        yield return new WaitForSeconds(.1f);
        yield return actor.StartCoroutine(actor.gameObject.LerpTo(targetCell.unitPosition, walkDuration));
    }
}
