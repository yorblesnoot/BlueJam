using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftModule : MonoBehaviour
{
    public List<CraftingSlot> craftingSlots;
    public EssenceSlot essenceSlot;

    public TMP_Text craftStatus;
    public TMP_Text essenceName;

    public Button craftButton;

    public virtual void ExecuteCraft(List<DraggableItem> craftingSlotContents, DraggableItem essenceSlotContents, RunData runData) { }

    public virtual void GetCraftSlotLimit(DraggableItem item)
    {
        
    }

    public virtual string GetCraftStatus(DraggableItem essenceSlotContents, List<DraggableItem> craftingSlotContents)
    {
        return "";
    }

    public virtual void FinalizeCraft(EssenceCrafting essenceCrafting)
    {
        essenceCrafting.gameObject.SetActive(false);
        gameObject.SetActive(false);
        EssenceCrafting.craftType = CraftType.BASE;
        essenceCrafting.gameObject.SetActive(true);
    }
}
