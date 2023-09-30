using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeCraft : CraftModule
{
    public override void ExecuteCraft(List<DraggableItem> craftingSlotContents, DraggableItem essenceSlotContents, RunData runData)
    {
        if (essenceSlotContents == null) return;
        int statFactor = craftingSlotContents.Count + 1;
        ModStat.Stat(StatType.DAMAGE, statFactor * 3, runData);
        ModStat.Stat(StatType.BARRIER, statFactor * 3, runData);
        ModStat.Stat(StatType.HEAL, statFactor * 3, runData);
    }

    public override string GetCraftStatus(DraggableItem essenceSlotContents, List<DraggableItem> craftingSlotContents)
    {
        if (essenceSlotContents == null) return "Insufficient materials!";
        else return $"Stat Boost Tier <color=#FF4E2B>{craftingSlotContents.Count + 1}</color>";
    }
}
