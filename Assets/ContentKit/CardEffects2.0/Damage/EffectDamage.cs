using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectDamage", menuName = "ScriptableObjects/CardEffects/Damage")]
public class EffectDamage : CardEffectPlus
{
    private void Reset()
    {
        effectClass = CardClass.ATTACK;
    }

    public override string GenerateDescription(IPlayerStats player)
    {
        return $"deal <color=#FF4E2B>{player.DamageScaling * scalingMultiplier}</color> damage";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit target in targets) Normal(actor, scalingMultiplier, target);
        yield return null;
    }
        //VFXMachine.PlayToLocation(effect, TurnManager.activeTurn.transform.position, targetCell.GetComponent<BattleTileController>().unitPosition);

    void Normal(BattleUnit owner, float damageMult, BattleUnit target)
    {
        int damage = Mathf.RoundToInt(owner.DamageScaling * damageMult);
        target.ReceiveDamage(damage);
    }
}
