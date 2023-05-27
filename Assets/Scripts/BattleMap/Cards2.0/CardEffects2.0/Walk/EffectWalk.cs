using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectWalk", menuName = "ScriptableObjects/CardEffects/Walk")]
public class EffectWalk : CardEffectPlus
{
    [Range(.1f, .01f)] public float stepSize;
    public override string GenerateDescription()
    {
        description = "Walk to the target cell";
        return description;
    }
    public override void Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        base.Execute(actor, targetCell, aoe);
        MonoBehaviour unitStats = actor.GetComponent<UnitActions>();
        unitStats.StartCoroutine(Walk(actor, targetCell, stepSize));
    }
    IEnumerator Walk(GameObject actor, GameObject destinationCell, float stepsize)
    {
        VFXMachine.AttachTrail("MoveTrail", actor);
        GridTools.ReportPositionChange(actor, destinationCell);
        Vector3 destination = destinationCell.GetComponent<BattleTileController>().unitPosition;
        while (actor.transform.position != destination)
        {
            actor.transform.position = Vector3.MoveTowards(actor.transform.position, destination, stepSize);
            yield return new WaitForSeconds(.01f);
        }
    }
}
