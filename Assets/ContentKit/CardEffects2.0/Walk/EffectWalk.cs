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
    bool walked;
    [Range(.1f, .01f)] public float stepSize;
    public override string GenerateDescription(IPlayerStats player)
    {
        return "move to target";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        MapTools.ReportPositionChange(actor, targetCell);
        yield return new WaitForSeconds(.1f);
        VFXMachine.AttachTrail("MoveTrail", actor.gameObject);
        Vector3 destination = targetCell.unitPosition;
        while (actor.transform.position != destination)
        {
            actor.transform.position = Vector3.MoveTowards(actor.transform.position, destination, stepSize);
            yield return new WaitForSeconds(.015f);
        }
    }
}
