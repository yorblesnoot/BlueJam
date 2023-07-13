using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingSlot : InventorySlot
{
    public EssenceCrafting essenceCrafting;
    public override void OnDrop(PointerEventData eventData)
    {
        essenceCrafting.ModifyCraftingSlotContents(ConfirmDrop(eventData), true);
    }
    public void EvictChildren()
    {
        if(transform.childCount > 0) 
        { 
            DraggableItem contents = transform.GetChild(0).gameObject.GetComponent<DraggableItem>();
            essenceCrafting.ModifyCraftingSlotContents(contents, false);
            essenceCrafting.PlaceStrayDraggable(contents);
        }
    }
}
