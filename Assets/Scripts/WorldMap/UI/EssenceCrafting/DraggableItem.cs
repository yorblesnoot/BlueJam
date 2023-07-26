using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    public Image image;
    public TMP_Text mySymbol;
    [HideInInspector]public EssenceCrafting essenceCrafting;

    [HideInInspector] public Deck essence;
    [HideInInspector] public Canvas mainCanvas;

    [SerializeField] InventorySlot originalSlot;

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
        
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
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
        screenPoint = new Vector3(screenPoint.x, screenPoint.y, 0);
        transform.localPosition = screenPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (essenceCrafting.essenceSlotContents != null) essenceCrafting.ShowEssenceDisplay(essenceCrafting.essenceSlotContents.essence);
        transform.SetParent(parentAfterDrag);
        if (parentAfterDrag.TryGetComponent<EssenceSlot>(out _)) essenceCrafting.EssenceSlotFilled(this);
        image.raycastTarget = true;
    }
}
