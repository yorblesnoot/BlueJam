using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneCraft : CraftModule
{
    public override bool ExecuteCraft(List<DraggableItem> craftingSlotContents, DraggableItem essenceSlotContents, RunData runData) 
    {
        if (!ClonePossible(essenceSlotContents)) return false;
        for (int i = 0; i < 2; i++)
        {
            runData.essenceInventory.Add(essenceSlotContents.essence);
            EssenceCrafting.flagList.Add(essenceSlotContents.essence);
        }
        return true;
    }

    public override string GetCraftStatus(DraggableItem essenceSlotContents, List<DraggableItem> craftingSlotContents)
    {
        if (!ClonePossible(essenceSlotContents)) return "Insufficient materials!";
        else return $"<color=#FF4E2B>Essence of {essenceSlotContents.essence.unitName} </color> will duplicate!";
    }

    bool ClonePossible(DraggableItem essenceSlotContents)
    {
        return (essenceSlotContents != null);
    }
}
