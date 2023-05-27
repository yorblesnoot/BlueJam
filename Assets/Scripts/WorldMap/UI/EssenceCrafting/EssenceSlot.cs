using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EssenceSlot : MonoBehaviour, IDropHandler
{
#nullable enable
    EssenceCrafting? essenceCrafting;
#nullable disable
    public void OnDrop(PointerEventData eventData)
    {
        if (ChildCountActive(transform) < 1)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem droppedItem = dropped.GetComponent<DraggableItem>();
            droppedItem.parentAfterDrag = transform;
            essenceCrafting = droppedItem.essenceCrafting;
            essenceCrafting.essenceSlot = droppedItem;
        }
    }

    public int ChildCountActive(Transform t)
    {
        int k = 0;
        foreach (Transform c in t)
        {
            if (c.gameObject.activeSelf)
                k++;
        }
        return k;
    }
}
