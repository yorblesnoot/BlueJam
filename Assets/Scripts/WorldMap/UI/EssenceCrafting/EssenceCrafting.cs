using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class EssenceCrafting : MonoBehaviour
{
    [SerializeField] RunData runData;
    private List<Deck> essences;
    public List<DraggableItem> dragItems;

    [SerializeField] TMP_Text description;
    [SerializeField] Canvas mainCanvas;

    [SerializeField] CardAwardUI cardAwardUI;

    [HideInInspector] public List<DraggableItem> craftingSlotContents;
    [HideInInspector] public DraggableItem essenceSlotContents;

    [SerializeField] List<CraftingSlot> craftingSlots;
    [SerializeField] TMP_Text craftStatus;

    [SerializeField] List<InventorySlot> inventorySlots;

    [SerializeField] List<MiniCardDisplay> miniCards;

    [SerializeField] WorldMenuControl worldMenuControl;
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
                dragItems[i].mySymbol.text = essences[i].symbol;
                dragItems[i].image.color = essences[i].iconColor;
            }
            else
            {
                dragItems[i].gameObject.SetActive(false);
            }
        }
    }

    public void EssenceSlotFilled(DraggableItem draggable = null)
    {
        int requiredSlots = 0;
        if(draggable != null)
        {
            //if an essence is in the main slot, set the default description to that essence
            requiredSlots = draggable.essence.deckContents.Count;
            ShowEssenceDisplay(draggable.essence);
        }
        essenceSlotContents = draggable;

        for (int i = 0; i < craftingSlots.Count; i++ )
        {
            if (i < requiredSlots)
            {
                craftingSlots[i].gameObject.SetActive(true);
                craftingSlots[i].essenceCrafting = this;
            }
            else
            {
                craftingSlots[i].EvictChildren();
                craftingSlots[i].gameObject.SetActive(false);
            }
        }
        
    }

    public void ModifyCraftingSlotContents(DraggableItem item, bool operation)
    {
        if (operation) craftingSlotContents.Add(item);
        else craftingSlotContents.Remove(item);
        string color = "<color=#FF4E2B>";
        if (craftingSlotContents.Count == 0) craftStatus.text = "Insufficient materials for crafting.";
        else if(craftingSlotContents.Count == 1) craftStatus.text = $"The current craft will offer {color}{craftingSlotContents.Count}</color> option from the essence above.";
        else craftStatus.text = $"The current craft will offer {color}{craftingSlotContents.Count}</color> options from the essence above.";
    }

    public void MergeButton()
    {
        //count the cards we're going to drop
        int dropCount = craftingSlotContents.Count;
        if (essenceSlotContents == null || dropCount == 0) return;
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
            int dropIndex = Random.Range(0, potentialDrops.Count - 1);
            actualDrops.Add(potentialDrops[dropIndex]);
            potentialDrops.RemoveAt(dropIndex);
        }

        EssenceSlotFilled();
        craftStatus.text = "Insufficient materials for crafting.";
        new SaveContainer(runData).SaveGame();
        cardAwardUI.gameObject.SetActive(true);
        cardAwardUI.AwardCards(actualDrops);

        SpendEssence(essenceSlotContents);
        for (int i = 0; i < craftingSlotContents.Count; i++)
        {
            SpendEssence(craftingSlotContents[i]);    
        }

        craftingSlotContents.Clear();
        essenceSlotContents = null;
        //worldMenuControl.ToggleWindow(gameObject, false);
    }

    public void SpendEssence(DraggableItem essence)
    {
        //remove the essences from the player's runData inventory and disable the draggables for the crafted items
        runData.essenceInventory.Remove(essence.essence);
        essence.gameObject.SetActive(false);
    }

    internal void PlaceStrayDraggable(DraggableItem contents)
    {
        Transform firstEmpty = inventorySlots.Where(x => x.ChildCountActive() == 0).First().transform;
        contents.parentAfterDrag = firstEmpty.transform;
        contents.transform.SetParent(firstEmpty.transform);
    }

    public void ShowEssenceDisplay(Deck essence)
    {
        essence.Initialize();
        description.text = essence.essenceDescription;
        int requiredSlots = 0;
        if (essence == null)
        {
            description.text = "";
        }
        else
        {
            description.text = essence.essenceDescription;
            requiredSlots = miniCards.Count;
        }
        for (int i = 0; i < requiredSlots; i++)
        {
            if (i < essence.deckContents.Count)
            {
                miniCards[i].gameObject.SetActive(true);
                miniCards[i].PopulateCard(essence.deckContents[i]);
            }
            else
            {
                miniCards[i].gameObject.SetActive(false);
            }
        }
    }
}
