using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectDamage", menuName = "ScriptableObjects/CardEffects/Damage")]
public class EffectDamage : CardEffectPlus
{

    //ImpactSmall, GroundBurst

    public override string GenerateDescription(IPlayerStats player)
    {
        return $"deal <color=#FF4E2B>{player.damageScaling * scalingMultiplier}</color> damage";
    }
    public override void ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit target in targets) Normal(actor, scalingMultiplier, target);
    }
        //VFXMachine.PlayToLocation(effect, TurnManager.activeTurn.transform.position, targetCell.GetComponent<BattleTileController>().unitPosition);

    void Normal(BattleUnit owner, float damageMult, BattleUnit target)
    {
        int damage = Mathf.RoundToInt(owner.damageScaling * damageMult);
        target.ReceiveDamage(damage);
    }
}
