using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class EssenceCrafting : MonoBehaviour
{
    public RunData runData;
    private List<Deck> essences;
    public List<DraggableItem> dragItems = new();
    

    public TMP_Text description;
    public Canvas mainCanvas;

    public CardAwardUI cardAwardUI;

    [HideInInspector] public List<DraggableItem> craftingSlots;
    [HideInInspector] public DraggableItem essenceSlot;

    private void Awake()
    {
        essences = new List<Deck>(runData.essenceInventory);
    }

    private void Start()
    {
        for( int i = 0; i < dragItems.Count; i++ )
        {
            if(i < essences.Count)
            {
                dragItems[i].essenceCrafting = this;
                dragItems[i].mainCanvas = mainCanvas;

                //associate a draggable with a specific deck
                dragItems[i].essence = essences[i];
                dragItems[i].description = description;
                dragItems[i].mySymbol.text = essences[i].symbol;
            }
            else
            {
                dragItems[i].gameObject.SetActive(false);
            }
        }
    }

    public void MergeButton()
    {
        //count the cards we're going to drop
        int dropCount = craftingSlots.Count;
        if (essenceSlot != null && dropCount > 0)
        {
            //create list of cards we'll drop
            List<CardPlus> actualDrops = new List<CardPlus>();

            //we are dropping from the deck in the essence slot
            Deck toDrop = essenceSlot.essence;

            //grab the list of possible card drops
            List<CardPlus> potentialDrops = new List<CardPlus>(toDrop.deckContents);

            //clamp the dropcount at the number of cards in the deck
            dropCount = Mathf.Clamp(dropCount, 0, potentialDrops.Count);

            for (int dropped = 0; dropped < dropCount; dropped++)
            {
                //pick a random card out of the potential drops and put it in the final drops
                int dropIndex = Random.Range(0, potentialDrops.Count - 1);
                actualDrops.Add(potentialDrops[dropIndex]);
                potentialDrops.RemoveAt(dropIndex);
            }
            cardAwardUI.gameObject.SetActive(true);
            cardAwardUI.AwardCards(actualDrops);

            SpendEssence(essenceSlot);
            for (int i = 0; i < craftingSlots.Count; i++)
            {
                SpendEssence(craftingSlots[i]);
                
            }

            gameObject.SetActive(false);
        }
    }

    public void SpendEssence(DraggableItem essence)
    {
        //remove the essences from the player's runData inventory
        runData.essenceInventory.Remove(essence.essence);

        //disable the draggables for the crafted items
        essence.gameObject.SetActive(false);
    }
}
