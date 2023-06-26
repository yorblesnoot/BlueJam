using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectWalk", menuName = "ScriptableObjects/CardEffects/Walk")]
public class EffectWalk : CardEffectPlus
{
    bool walked;
    [Range(.1f, .01f)] public float stepSize;
    public override string GenerateDescription(IPlayerData player)
    {
        return "move to target";
    }
    public override void ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        doneExecuting = false;
        actor.StartCoroutine(Walk(actor, targetCell, stepSize));
    }
    IEnumerator Walk(BattleUnit actor, BattleTileController destinationCell, float stepsize)
    {
        GridTools.ReportPositionChange(actor, destinationCell);
        yield return new WaitForSeconds(.1f);
        VFXMachine.AttachTrail("MoveTrail", actor.gameObject);
        Vector3 destination = destinationCell.unitPosition;
        while (actor.transform.position != destination)
        {
            actor.transform.position = Vector3.MoveTowards(actor.transform.position, destination, stepSize);
            yield return new WaitForSeconds(.015f);
        }
        doneExecuting = true;
    }
}
