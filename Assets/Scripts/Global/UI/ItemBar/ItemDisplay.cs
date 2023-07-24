using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public BattleItem battleItem;
    [SerializeField] TMP_Text tooltipText;
    [SerializeField] GameObject tooltip;
    public Image thumbnail;

    public virtual void DisplayItem(BattleItem item)
    {
        battleItem = item;
        thumbnail.sprite = item.thumbnail;
        thumbnail.color = item.thumbnailColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true);
        //tooltip.transform.SetParent(null, true);
        tooltipText.text = battleItem.description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
}
