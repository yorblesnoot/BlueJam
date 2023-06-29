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
    [HideInInspector]public TMP_Text description;

    [HideInInspector] public Deck essence;
    [HideInInspector] public Canvas mainCanvas;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        essence.Initialize();
        description.text = essence.essenceDescription;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
        if (essenceCrafting.craftingSlots.Contains(this))
        {
            essenceCrafting.craftingSlots.Remove(this);
        }
        if (essenceCrafting.essenceSlot == this)
        {
            essenceCrafting.essenceSlot = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hit;
        Physics.Raycast(ray, out Hit);
        Vector3 screenPoint = Hit.point;
        screenPoint = mainCanvas.transform.InverseTransformPoint(screenPoint);
        screenPoint = new Vector3(screenPoint.x, screenPoint.y, 0);
        transform.localPosition = screenPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}
