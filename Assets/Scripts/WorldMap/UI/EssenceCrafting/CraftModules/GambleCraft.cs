using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GambleCraft : CraftModule
{
    [SerializeField] LoadLibrary loadLibrary;
    public override void ExecuteCraft(List<DraggableItem> craftingSlotContents, DraggableItem essenceSlotContents, RunData runData)
    {
        if (essenceSlotContents == null) return;
        int poolChoice = Random.Range(0, 5 - craftingSlotContents.Count);
        List<Deck> finalPool = new();
        if(poolChoice == 0) finalPool = loadLibrary.decksPool.Where(x => x.deckContents.Count >= 5 && x.deckContents.Count <= 6).ToList();
        else finalPool = loadLibrary.decksPool.Where(x => x.deckContents.Count <= 4).ToList();

        int deckChoice = Random.Range(0, finalPool.Count);
        runData.essenceInventory.Add(finalPool[deckChoice]);
    }

    public override string GetCraftStatus(DraggableItem essenceSlotContents, List<DraggableItem> craftingSlotContents)
    {
        if (essenceSlotContents == null) return "Insufficient materials!";
        else return $"<color=#FF4E2B>{craftingSlotContents.Count * 25}%</color> Chance of Boss Essence";
    }
}
