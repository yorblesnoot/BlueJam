using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class EssenceCrafting : MonoBehaviour
{
    [SerializeField] RunData runData;
    public List<DraggableItem> dragItems;

    [SerializeField] List<InventorySlot> inventorySlots;
    [SerializeField] List<CraftingSlot> craftingSlots;
    [SerializeField] EssenceSlot essenceSlot;
    [SerializeField] List<MiniCardDisplay> miniCards;
    [SerializeField] WrldCardDisplay bigCardDisplay;

    [HideInInspector] public List<DraggableItem> craftingSlotContents;
    [HideInInspector] public DraggableItem essenceSlotContents;

    [SerializeField] TMP_Text craftStatus;
    [SerializeField] TMP_Text description;
    [SerializeField] Canvas mainCanvas;
    

    [SerializeField] CardAwardUI cardAwardUI;
    [SerializeField] WorldMenuControl worldMenuControl;
    

    readonly int hatTiltAngle = -10;
    readonly int hatProjectionDistance = 30;
    private void Awake()
    {
        foreach(var card in miniCards)
        {
            card.bigCard = bigCardDisplay;
        }
        runData.essenceInventory = runData.essenceInventory.OrderBy(x => x.name).ToList();
        for( int i = 0; i < dragItems.Count; i++ )
        {
            dragItems[i].transform.SetParent(inventorySlots[i].transform, false);
            if(i < runData.essenceInventory.Count)
            {
                dragItems[i].gameObject.SetActive(true);
                dragItems[i].essenceCrafting = this;
                dragItems[i].mainCanvas = mainCanvas;

                //associate a draggable with a specific deck
                dragItems[i].essence = runData.essenceInventory[i];

                GameObject hat = Instantiate(runData.essenceInventory[i].hat);
                hat.transform.SetParent(dragItems[i].transform, false);
                

                Transform scalingCube = hat.transform.GetChild(0);
                scalingCube.transform.SetParent(dragItems[i].transform, true);
                hat.transform.SetParent(scalingCube, true);

                scalingCube.transform.localScale = Vector3.one * 50;
                Vector3 position = new(0, 0, -hatProjectionDistance);
                scalingCube.transform.localPosition = position;
                scalingCube.transform.localRotation = Quaternion.identity;
                hat.transform.localRotation = Quaternion.Euler(hatTiltAngle,0,0);
            }
            else
            {
                dragItems[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        EssenceSlotFilled();
        for (int i = 0; i < dragItems.Count; i++)
        {
            dragItems[i].transform.SetParent(inventorySlots[i].transform, false);
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
        if (craftingSlotContents.Count == 0) craftStatus.text = "Insufficient materials!";
        else craftStatus.text = $"<color=#FF4E2B>{craftingSlotContents.Count}</color> {essenceSlotContents.essence.unitName} Card{(craftingSlotContents.Count > 1 ? "s" : "")}";
    }

    public void MergeButton()
    {
        //count the cards we're going to drop
        int dropCount = craftingSlotContents.Count;
        if (essenceSlotContents == null || dropCount == 0) return;
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

        PlaceStrayDraggable(essenceSlotContents);
        SpendEssence(essenceSlotContents);
        for (int i = 0; i < craftingSlotContents.Count; i++)
        {
            SpendEssence(craftingSlotContents[i]);    
        }
        craftingSlotContents.Clear();
        essenceSlotContents = null;
        EssenceSlotFilled();
        craftStatus.text = "Insufficient materials!";
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
        contents.transform.localScale = Vector3.one;
    }

    public void ShowEssenceDisplay(Deck essence)
    {
        essence.Initialize();
        int requiredSlots = 0;
        if (essence == null)
        {
            description.text = "~";
        }
        else
        {
            description.text = essence.unitName;
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

    public void QuickCraft(DraggableItem item)
    {
        if (essenceSlotContents == item) 
        {
            EssenceSlotFilled();
            PlaceStrayDraggable(item);
            return;
        }

        if (craftingSlotContents.Contains(item))
        {
            ModifyCraftingSlotContents(item, false);
            PlaceStrayDraggable(item);
            return;
        }

        if(essenceSlotContents == null)
        {
            item.transform.SetParent(essenceSlot.transform, false);
            item.transform.localScale = Vector3.one;
            EssenceSlotFilled(item);
            return;
        }

        if(craftingSlotContents.Count < (essenceSlotContents ? essenceSlotContents.essence.deckContents.Count : 0))
        {
            Transform firstEmpty = craftingSlots.Where(x => x.ChildCountActive() == 0).First().transform;
            item.transform.SetParent(firstEmpty, false);
            ModifyCraftingSlotContents(item, true);
        }
    }
}
