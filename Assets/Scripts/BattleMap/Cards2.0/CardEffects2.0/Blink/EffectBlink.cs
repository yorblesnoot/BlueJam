using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectBlink", menuName = "ScriptableObjects/CardEffects/Blink")]
public class EffectBlink : CardEffectPlus
{
    public override string GenerateDescription()
    {
        description = $"Blink to the target cell";
        return description;
    }
    public override List<GameObject> Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        base.Execute(actor, targetCell, aoe);
        GridTools.ReportPositionChange(actor, targetCell);
        Vector3 destination = targetCell.GetComponent<BattleTileController>().unitPosition;
        actor.transform.position = destination;
        return null;
    }
}
