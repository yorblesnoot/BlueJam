public class EssenceSlot : InventorySlot
{
    public override void ReceiveDraggable(DraggableItem item)
    {
        base.ReceiveDraggable(item);
        essenceCrafting.EssenceSlotFilled(item);
    }
}
