using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCraft : CraftModule
{
    [SerializeField] CardAwardUI cardAwardUI;
    public override void ExecuteCraft(List<DraggableItem> craftingSlotContents, DraggableItem essenceSlotContents, RunData runData)
    {
        //count the cards we're going to drop
        int dropCount = craftingSlotContents.Count;
        if (essenceSlotContents == null || dropCount == 0 || WorldPlayerControl.playerState == WorldPlayerState.SELECTION) return;
        SoundManager.PlaySound(SoundType.CRAFTCONFIRMED);
        //create list of cards we'll drop
        List<CardPlus> actualDrops = new();

        //we are dropping from the deck in the essence slot
        Deck toDrop = essenceSlotContents.essence;

        //grab the list of possible card drops
        List<CardPlus> potentialDrops = new(toDrop.deckContents);

        //clamp the dropcount at the number of cards in the deck
        dropCount = Mathf.Clamp(dropCount, 0, potentialDrops.Count);

        for (int dropped = 0; dropped < dropCount; dropped++)
        {
            //pick a random card out of the potential drops and put it in the final drops
            int dropIndex = Random.Range(0, potentialDrops.Count);
            actualDrops.Add(potentialDrops[dropIndex]);
            potentialDrops.RemoveAt(dropIndex);
        }

        new SaveContainer(runData).SaveGame();
        cardAwardUI.gameObject.SetActive(true);
        cardAwardUI.AwardCards(actualDrops);
    }

    public override void GetCraftSlotLimit(DraggableItem item)
    {
        int requiredSlots = item ? item.essence.deckContents.Count : 0;
        for (int i = 0; i < craftingSlots.Count; i++)
        {
            if (i < requiredSlots)
            {
                craftingSlots[i].gameObject.SetActive(true);
            }
            else
            {
                craftingSlots[i].EvictChildren();
                craftingSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public override string GetCraftStatus(DraggableItem essenceSlotContents, List<DraggableItem> craftingSlotContents)
    {
        if (craftingSlotContents.Count == 0 || essenceSlotContents == null) return "Insufficient materials!";
        else return $"<color=#FF4E2B>{craftingSlotContents.Count}</color> {essenceSlotContents.essence.unitName} Card{(craftingSlotContents.Count > 1 ? "s" : "")}";
    }

    public override void FinalizeCraft(EssenceCrafting essenceCrafting) { }
}
