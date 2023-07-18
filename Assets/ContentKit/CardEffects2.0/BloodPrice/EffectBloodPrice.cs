using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectBloodPrice", menuName = "ScriptableObjects/CardEffects/BloodPrice")]
public class EffectBloodPrice : CardEffectPlus
{
    private void Reset()
    {
        effectClass = CardClass.BUFF;
    }

    public override string GetEffectDescription(IPlayerStats player)
    {
        return $"pay <color=red>{player.maxHealth * scalingMultiplier/100}</color> life";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        actor.ReduceHealth(Mathf.RoundToInt(actor.maxHealth * scalingMultiplier/100));
        yield return null;
    }
}
