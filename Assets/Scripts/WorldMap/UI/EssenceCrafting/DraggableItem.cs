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
    EssenceCrafting essenceCrafting;
    [HideInInspector] public Deck essence;
    Canvas mainCanvas;

    public HatControl hatControl;

    [SerializeField] ParticleSystem newFire;

    readonly int thrustDistance = 50;
    public void InitializeDraggable(EssenceCrafting crafter, Canvas canvas, Deck deck)
    {
        gameObject.SetActive(true);
        essenceCrafting = crafter;
        mainCanvas = canvas;
        essence = deck;

        ColorUtility.TryParseHtmlString("#F8B63C", out Color colorFromHex);
        if (deck.deckContents.Count >= 5) image.color = colorFromHex;
        else image.color = Color.white;

        hatControl ??= new(image);
        hatControl.DeployHat(deck.hat);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        newFire.Stop();
        if (essenceCrafting.essenceSlotContents == null)
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
        RemoveFromCraftSlot();

        essenceCrafting.ShowEssenceDisplay(essence);
    }

    public void RemoveFromCraftSlot()
    {
        if (essenceCrafting.craftingSlotContents.Contains(this))
        {
            essenceCrafting.ModifyCraftingSlotContents(this, false);
        }
        if (essenceCrafting.essenceSlotContents == this)
        {
            essenceCrafting.EssenceSlotFilled();
        }
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

    public void HighlightAsNew()
    {
        newFire.Play();
    }
}
