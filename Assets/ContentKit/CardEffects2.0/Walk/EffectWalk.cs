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
    public override List<BattleUnit> Execute(BattleUnit actor, BattleTileController targetCell, string[,] aoe)
    {
        base.Execute(actor, targetCell, aoe);
        doneExecuting = false;
        actor.StartCoroutine(Walk(actor, targetCell, stepSize));
        return null;
    }
    IEnumerator Walk(BattleUnit actor, BattleTileController destinationCell, float stepsize)
    {
        yield return new WaitForSeconds(.1f);
        VFXMachine.AttachTrail("MoveTrail", actor.gameObject);
        GridTools.ReportPositionChange(actor, destinationCell);
        Vector3 destination = destinationCell.unitPosition;
        while (actor.transform.position != destination)
        {
            actor.transform.position = Vector3.MoveTowards(actor.transform.position, destination, stepSize);
            yield return new WaitForSeconds(.015f);
        }
        doneExecuting = true;
    }
}
