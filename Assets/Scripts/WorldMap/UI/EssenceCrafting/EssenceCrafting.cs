using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum CraftType
{
    BASE,
    CLONE,
    CONSUME,
    GAMBLE
}
public class EssenceCrafting : MonoBehaviour
{
    [System.Serializable]
    class CraftPackage
    {
        public CraftType type;
        public CraftModule module;
    }
    [SerializeField] List<CraftPackage> packages;
    Dictionary<CraftType, CraftModule> modules;
    public static CraftType craftType;

    [SerializeField] RunData runData;
    public List<DraggableItem> dragItems;

    [SerializeField] List<InventorySlot> inventorySlots;


    [SerializeField] List<MiniCardDisplay> miniCards;
    [SerializeField] WrldCardDisplay bigCardDisplay;

    [HideInInspector] public List<DraggableItem> craftingSlotContents;
    [HideInInspector] public DraggableItem essenceSlotContents;

    [SerializeField] Canvas mainCanvas;

    public static UnityEvent craftWindowClosed = new();

    public static List<Deck> flagList;

    private void Awake()
    {
        flagList = new();
        modules = new();
        foreach (var item in packages)
        {
            modules.Add(item.type, item.module);
        }
    }
    private void OnEnable()
    {
        modules[craftType].craftButton.onClick.AddListener(MergeButton);
        modules[CraftType.BASE].gameObject.SetActive(false);
        modules[craftType].gameObject.SetActive(true);

        List<InventorySlot> slots = new();
        slots.AddRange(inventorySlots);
        slots.AddRange(modules[craftType].craftingSlots);
        slots.Add(modules[craftType].essenceSlot);

        foreach (var slot in slots) slot.essenceCrafting = this;
        foreach (var card in miniCards) card.bigCard = bigCardDisplay;

        runData.essenceInventory = runData.essenceInventory.OrderBy(x => x.name).ToList();
        for (int i = 0; i < dragItems.Count; i++)
        {
            dragItems[i].transform.SetParent(inventorySlots[i].transform, false);
            if (i < runData.essenceInventory.Count)
            {
                dragItems[i].gameObject.SetActive(true);
                dragItems[i].essenceCrafting = this;
                dragItems[i].mainCanvas = mainCanvas;

                //associate a draggable with a specific deck
                Deck deck = runData.essenceInventory[i];
                dragItems[i].essence = deck;
                if (flagList.Contains(deck))
                {
                    dragItems[i].HighlightAsNew();
                    flagList.Remove(deck);
                }

                ColorUtility.TryParseHtmlString("#F8B63C", out Color colorFromHex);
                if (runData.essenceInventory[i].deckContents.Count >= 5) dragItems[i].image.color = colorFromHex;
                else dragItems[i].image.color = Color.white;

                dragItems[i].DeployHat();
            }
            else
            {
                dragItems[i].gameObject.SetActive(false);
            }
        }
        Tutorial.CompleteStage(TutorialFor.WORLDCRAFTING, 1);
        Tutorial.CompleteStage(TutorialFor.WORLDCRAFTREMINDER, 1);
        Tutorial.EnterStage(TutorialFor.WORLDCRAFTING, 2,
            "Drag an essence you want cards from into the <color=blue>top slot</color>, then drag essences you don't want into the <color=blue>bottom slots.</color>");
        EssenceSlotFilled();
        for (int i = 0; i < dragItems.Count; i++)
        {
            dragItems[i].transform.SetParent(inventorySlots[i].transform, false);
        }
    }

    private void OnDisable()
    {
        essenceSlotContents = null;
        craftingSlotContents.Clear();
        modules[craftType].gameObject.SetActive(false);
        modules[craftType].craftButton.onClick.RemoveAllListeners();
        craftType = CraftType.BASE;
        craftWindowClosed?.Invoke();
    }

    public void EssenceSlotFilled(DraggableItem draggable = null)
    {
        if(draggable != null)
        {
            //if an essence is in the main slot, set the default description to that essence
            ShowEssenceDisplay(draggable.essence);
        }
        essenceSlotContents = draggable;
        modules[craftType].GetCraftSlotLimit(essenceSlotContents);
        modules[craftType].craftStatus.text = modules[craftType].GetCraftStatus(essenceSlotContents, craftingSlotContents);
    }

    public void ModifyCraftingSlotContents(DraggableItem item, bool operation)
    {
        Tutorial.CompleteStage(TutorialFor.WORLDCRAFTING, 2);
        Tutorial.EnterStage(TutorialFor.WORLDCRAFTING, 3,
            "You can see the cards available from an essence in the bottom right. <color=blue>For each essence</color> you add to the craft, you'll be <color=green>offered an additional option</color> to choose from. Press the hammer button to craft.");
        if (operation) craftingSlotContents.Add(item);
        else craftingSlotContents.Remove(item);

        modules[craftType].craftStatus.text = modules[craftType].GetCraftStatus(essenceSlotContents, craftingSlotContents);
    }

    public void MergeButton()
    {
        if(!modules[craftType].ExecuteCraft(craftingSlotContents, essenceSlotContents, runData)) return;

        PlaceStrayDraggable(essenceSlotContents);
        SpendEssence(essenceSlotContents);
        for (int i = 0; i < craftingSlotContents.Count; i++)
        {
            SpendEssence(craftingSlotContents[i]);
        }
        craftingSlotContents.Clear();
        essenceSlotContents = null;
        EssenceSlotFilled();
        modules[craftType].craftStatus.text = "Craft Complete!";
        modules[craftType].FinalizeCraft(this);
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
            modules[craftType].essenceName.text = "~";
        }
        else
        {
            modules[craftType].essenceName.text = essence.unitName;
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
        SoundManager.PlaySound(SoundType.INVENTORYGRAB);
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
            item.transform.SetParent(modules[craftType].essenceSlot.transform, false);
            item.transform.localScale = Vector3.one;
            EssenceSlotFilled(item);
            return;
        }

        if(craftingSlotContents.Count < modules[craftType].craftingSlots.Where(x => x.gameObject.activeSelf).Count())
        {
            Transform firstEmpty = modules[craftType].craftingSlots.Where(x => x.ChildCountActive() == 0).First().transform;
            item.transform.SetParent(firstEmpty, false);
            ModifyCraftingSlotContents(item, true);
        }
    }
}
