using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneCraft : CraftModule
{
    public override void ExecuteCraft(List<DraggableItem> craftingSlotContents, DraggableItem essenceSlotContents, RunData runData) 
    {
        if (essenceSlotContents == null || craftingSlotContents.Count < 2) return;
        for(int i = 0; i < 2; i++) runData.essenceInventory.Add(essenceSlotContents.essence);
    }

    public override string GetCraftStatus(DraggableItem essenceSlotContents, List<DraggableItem> craftingSlotContents)
    {
        if (craftingSlotContents.Count < 2 || essenceSlotContents == null) return "Insufficient materials!";
        else return $"<color=#FF4E2B>Essence of {essenceSlotContents.essence.unitName} </color> will duplicate!";
    }
}
