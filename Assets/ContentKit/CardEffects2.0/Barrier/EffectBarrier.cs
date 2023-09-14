using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectBarrier", menuName = "ScriptableObjects/CardEffects/Barrier")]
public class EffectBarrier : CardEffectPlus
{
    enum BarrierType { DEFLECT, SHIELD }
    [SerializeField] BarrierType barrierType;

    private void Reset()
    {
        effectSound = SoundTypeEffect.BARRIER;
        effectClass = CardClass.BUFF;
    }

    readonly Dictionary<BarrierType, string> barrierNames = new()
    {
        {BarrierType.DEFLECT, "block" },
        {BarrierType.SHIELD, "shield" }
    };

    public override string GetEffectDescription(Unit player)
    {
        return $"{barrierNames[barrierType]} for <color=#1ED5FA>{Mathf.RoundToInt(player.loadedStats[StatType.BARRIER] * scalingMultiplier)}</color>";
    }
    public override IEnumerator ActivateEffect(BattleUnit actor, BattleTileController targetCell, bool[,] aoe = null, List<BattleUnit> targets = null)
    {
        foreach (BattleUnit targetUnit in targets)
        {
            BarrierTracker barrierTracker = targetUnit.gameObject.GetComponent<BarrierTracker>();
            int barrier = Mathf.RoundToInt(actor.loadedStats[StatType.BARRIER] * scalingMultiplier);
            if (barrierType == BarrierType.DEFLECT) barrierTracker.AddDeflect(barrier);
            else if (barrierType == BarrierType.SHIELD) barrierTracker.AddShield(barrier);
        }
        yield return null;
    }
}
