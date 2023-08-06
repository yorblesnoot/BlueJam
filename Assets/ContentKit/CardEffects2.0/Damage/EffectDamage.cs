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

    public override string GetEffectDescription(Unit player)
    {
        return $"deal <color=#FF4E2B>{Mathf.RoundToInt(player.loadedStats[StatType.DAMAGE] * scalingMultiplier)}</color> damage";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit target in targets) Normal(actor, scalingMultiplier, target);
        yield return null;
    }

    void Normal(BattleUnit owner, float damageMult, BattleUnit target)
    {
        int damage = Mathf.RoundToInt(owner.loadedStats[StatType.DAMAGE] * damageMult);
        target.ReceiveDamage(damage);
    }
}
