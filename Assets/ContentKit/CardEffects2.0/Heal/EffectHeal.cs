using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectHeal", menuName = "ScriptableObjects/CardEffects/Heal")]
public class EffectHeal : CardEffectPlus
{
    private void Reset()
    {
        effectSound = SoundTypeEffect.HEAL;
        effectClass = CardClass.BUFF;
    }

    public override string GetEffectDescription(Unit player)
    {
        return $"heal for <color=#1EFA61>{Mathf.RoundToInt(player.loadedStats[StatType.HEAL] * scalingMultiplier)}</color>";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit target in targets) Heal(actor, scalingMultiplier, target);
        yield return null ;
    }

    void Heal(BattleUnit owner, float healMult, BattleUnit target)
    {
        int heal = Mathf.RoundToInt(owner.loadedStats[StatType.HEAL] * healMult);
        target.HealHealth(heal);
    }
}
