public class CraftingSlot : InventorySlot
{
    public override void ReceiveDraggable(DraggableItem item)
    {
        base.ReceiveDraggable(item);
        essenceCrafting.ModifyCraftingSlotContents(item, true);
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
