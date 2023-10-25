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
    [HideInInspector] public EssenceCrafting essenceCrafting;

    [HideInInspector] public Deck essence;
    [HideInInspector] public Canvas mainCanvas;

    [SerializeField] ParticleSystem newFire;

    [HideInInspector] public GameObject hat;

    readonly int thrustDistance = 50;
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

    readonly static int hatTiltAngle = -10;
    readonly static int hatProjectionDistance = 30;
    public void DeployHat()
    {
        if(hat != null) Destroy(hat);
        hat = Instantiate(essence.hat);
        hat.transform.SetParent(image.transform, false);

        Transform scalingCube = hat.transform.GetChild(0);
        scalingCube.transform.SetParent(image.transform, true);
        hat.transform.SetParent(scalingCube, true);

        scalingCube.transform.localScale = Vector3.one * 50;
        Vector3 position = new(0, 0, -hatProjectionDistance);
        scalingCube.transform.localPosition = position;
        scalingCube.transform.localRotation = Quaternion.identity;
        hat.transform.localRotation = Quaternion.Euler(hatTiltAngle, 0, 0);
    }

    public void HighlightAsNew()
    {
        newFire.Play();
    }
}
