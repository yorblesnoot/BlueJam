using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public EssenceCrafting essenceCrafting;
    public virtual void OnDrop(PointerEventData eventData)
    {
        SoundManager.PlaySound(SoundType.INVENTORYDROP);
        
        GameObject dropped = eventData.pointerDrag;
        DraggableItem droppedItem = dropped.GetComponent<DraggableItem>();
        int children = ChildCountActive();
        if (children >= 1)
        {
            //get the parent of the incoming object
            Transform otherParent = droppedItem.parentAfterDrag;
            Transform childDraggable = transform.GetChild(0);
            childDraggable.SetParent(otherParent);
            childDraggable.localScale = Vector3.one;
            otherParent.gameObject.GetComponent<InventorySlot>().ReceiveDraggable(childDraggable.gameObject.GetComponent<DraggableItem>());
        }
        droppedItem.parentAfterDrag = transform;
        ReceiveDraggable(droppedItem);
    }

    public virtual void ReceiveDraggable(DraggableItem item)
    {
        item.RemoveFromCraftSlot();
    }

    public int ChildCountActive()
    {
        int k = 0;
        foreach (Transform c in transform)
        {
            if (c.gameObject.activeSelf)
                k++;
        }
        return k;
    }
}
