using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectDamage", menuName = "ScriptableObjects/CardEffects/Damage")]
public class EffectDamage : CardEffectPlus
{

    //ImpactSmall, GroundBurst

    public override string GenerateDescription()
    {
        description = "Deal [damage] damage";
        return description;
    }
    public override List<BattleUnit> Execute(BattleUnit actor, BattleTileController targetCell, string[,] aoe)
    {
        List<BattleUnit> targets = base.Execute(actor, targetCell, aoe);
        foreach (BattleUnit target in targets) Normal(actor, scalingMultiplier, target);
        return targets;
    }
        //VFXMachine.PlayToLocation(effect, TurnManager.activeTurn.transform.position, targetCell.GetComponent<BattleTileController>().unitPosition);

    void Normal(BattleUnit owner, float damageMult, BattleUnit target)
    {
        int damage = Mathf.RoundToInt(owner.damageScaling * damageMult);
        target.ReceiveDamage(damage);
    }
}
