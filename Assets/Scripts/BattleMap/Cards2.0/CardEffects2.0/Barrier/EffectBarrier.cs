using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectBarrier", menuName = "ScriptableObjects/CardEffects/Barrier")]
public class EffectBarrier : CardEffectPlus
{
    enum BarrierType { DEFLECT, SHIELD }
    [SerializeField] BarrierType barrierType;

    public override string GenerateDescription()
    {
        if (barrierType == BarrierType.DEFLECT)
            description = "Deflect for [barrier]";
        else if (barrierType == BarrierType.SHIELD)
            description = "Shield for [barrier]";
        return description;
    }
    public override List<GameObject> Execute(GameObject actor, GameObject targetCell, string[,] aoe)
    {
        List < GameObject > targets = base.Execute(actor, targetCell, aoe);
        foreach (GameObject targetUnit in targets)
        {
            BarrierTracker barrierTracker = targetUnit.GetComponent<BarrierTracker>();
            UnitActions stats = targetUnit.GetComponent<UnitActions>();
            int barrier = Mathf.RoundToInt(stats.barrierScaling * scalingMultiplier);
            if (barrierType == BarrierType.DEFLECT) barrierTracker.AddDeflect(barrier);
            else if (barrierType == BarrierType.SHIELD) barrierTracker.AddShield(barrier);
        }
        return null;
    }
}
