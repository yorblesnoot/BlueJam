using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public virtual void OnDrop(PointerEventData eventData)
    {
        ConfirmDrop(eventData);
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
    public DraggableItem ConfirmDrop(PointerEventData eventData)
    {
        int children = ChildCountActive();
        GameObject dropped = eventData.pointerDrag;
        DraggableItem droppedItem = dropped.GetComponent<DraggableItem>();
        if (children >= 1)
        {
            //get the parent of the incoming object
            Transform otherParent = droppedItem.parentAfterDrag;
            transform.GetChild(0).SetParent(otherParent);
        }
        droppedItem.parentAfterDrag = transform;       
        return droppedItem;
    }
}
