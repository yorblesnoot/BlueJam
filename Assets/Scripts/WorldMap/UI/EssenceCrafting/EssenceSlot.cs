using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EssenceSlot : InventorySlot
{
    [SerializeField] EssenceCrafting essenceCrafting;
    public override void OnDrop(PointerEventData eventData)
    {
        essenceCrafting.EssenceSlotFilled(ConfirmDrop(eventData));
    }
}
