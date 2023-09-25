using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerClickHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    public Image image;
    [HideInInspector]public EssenceCrafting essenceCrafting;

    [HideInInspector] public Deck essence;
    [HideInInspector] public Canvas mainCanvas;

    [SerializeField] InventorySlot originalSlot;

    readonly int thrustDistance = 50;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(essenceCrafting.essenceSlotContents == null)
        {
            essence.Initialize();
            essenceCrafting.ShowEssenceDisplay(essence);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SoundManager.PlaySound(SoundType.INVENTORYGRAB);
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        transform.localScale = Vector3.one;
        image.raycastTarget = false;
        if (essenceCrafting.craftingSlotContents.Contains(this))
        {
            essenceCrafting.ModifyCraftingSlotContents(this, false);
        }
        if (essenceCrafting.essenceSlotContents == this)
        {
            essenceCrafting.EssenceSlotFilled();
        }
        essenceCrafting.ShowEssenceDisplay(essence);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit Hit);
        Vector3 screenPoint = Hit.point;
        screenPoint = mainCanvas.transform.InverseTransformPoint(screenPoint);
        screenPoint = new Vector3(screenPoint.x, screenPoint.y, -thrustDistance);
        transform.localPosition = screenPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (essenceCrafting.essenceSlotContents != null) essenceCrafting.ShowEssenceDisplay(essenceCrafting.essenceSlotContents.essence);
        transform.SetParent(parentAfterDrag, false);
        transform.localPosition = Vector3.zero;
        if (parentAfterDrag.TryGetComponent<EssenceSlot>(out _)) essenceCrafting.EssenceSlotFilled(this);
        image.raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            essenceCrafting.QuickCraft(this);
        }
    }
}
