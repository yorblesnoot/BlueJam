using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectHeal", menuName = "ScriptableObjects/CardEffects/Heal")]
public class EffectHeal : CardEffectPlus
{

    //GreenHeal, HealBurst

    public override string GenerateDescription(IPlayerData player)
    {
        return $"heal for <color=#1EFA61>{player.healScaling * scalingMultiplier}</color>";
    }
    public override List<BattleUnit> Execute(BattleUnit actor, BattleTileController targetCell, string[,] aoe)
    {
        List<BattleUnit> targets = base.Execute(actor, targetCell, aoe);
        foreach (BattleUnit target in targets) Heal(actor, scalingMultiplier, target);
        return targets;
    }
    //VFXMachine.PlayToLocation(effect, TurnManager.activeTurn.transform.position, targetCell.GetComponent<BattleTileController>().unitPosition);

    void Heal(BattleUnit owner, float healMult, BattleUnit target)
    {
        int heal = -Mathf.RoundToInt(owner.healScaling * healMult);
        target.ReceiveDamage(heal);
    }
}
