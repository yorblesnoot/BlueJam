using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BarItemDisplay : ItemDisplay, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text tooltipText;
    [SerializeField] GameObject tooltip;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip)
            tooltip.SetActive(true);
        //tooltip.transform.SetParent(null, true);
        tooltipText.text = battleItem.description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
}
