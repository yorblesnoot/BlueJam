using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectStall", menuName = "ScriptableObjects/CardEffects/Stall")]
public class EffectStall : CardEffectPlus
{
    private void Reset()
    {
        effectClass = CardClass.ATTACK;
        targetNotRequired = true;
    }

    public override string GetEffectDescription(Unit player)
    {
        return "";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        yield return new WaitForSeconds(scalingMultiplier);
    }
}