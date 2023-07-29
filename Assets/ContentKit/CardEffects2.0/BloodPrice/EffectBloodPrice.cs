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
        return $"pay <color=red>{Mathf.RoundToInt(player.loadedStats[StatType.MAXHEALTH] * scalingMultiplier/100)}</color> life";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        actor.ModifyHealth(Mathf.RoundToInt(actor.loadedStats[StatType.MAXHEALTH] * scalingMultiplier/100));
        yield return null;
    }
}
