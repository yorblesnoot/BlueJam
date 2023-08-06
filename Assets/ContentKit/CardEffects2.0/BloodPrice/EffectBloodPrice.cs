using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectBloodPrice", menuName = "ScriptableObjects/CardEffects/BloodPrice")]
public class EffectBloodPrice : CardEffectPlus
{
    private void Reset()
    {
        effectSound = SoundTypeEffect.BLOODPRICE;
        effectClass = CardClass.BUFF;
    }

    public override string GetEffectDescription(Unit player)
    {
        return $"pay {scalingMultiplier}% (<color=red>{Mathf.RoundToInt(scalingMultiplier / 100 * player.currentHealth)}</color>) of your current life";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        actor.ModifyHealth(Mathf.RoundToInt(scalingMultiplier/100 * actor.currentHealth));
        yield return null;
    }
}
