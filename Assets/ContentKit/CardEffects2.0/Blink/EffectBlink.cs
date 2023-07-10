using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EffectBlink", menuName = "ScriptableObjects/CardEffects/Blink")]
public class EffectBlink : CardEffectPlus
{
    private void Reset()
    {
        effectClass = CardClass.MOVE;
    }
    [SerializeField] float delay;
    public override string GenerateDescription(IPlayerStats player)
    {
        return $"blink to target";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        MapTools.ReportPositionChange(actor, targetCell);
        Vector3 destination = targetCell.unitPosition;
        actor.transform.position = new Vector3(100, 100, 100);
        yield return new WaitForSeconds(delay);
        actor.transform.position = destination;
    }
}
