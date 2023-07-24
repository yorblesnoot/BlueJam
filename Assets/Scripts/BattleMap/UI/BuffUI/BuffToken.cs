using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffToken : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text stackDisplay;
    [SerializeField] Image myImage;
    [SerializeField] TMP_Text tooltipText;
    [SerializeField] GameObject tooltip;
    string description;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true);
        tooltipText.text = description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }

    public void RenderBuff(Color32 buffColor, int buffStack, string description)
    {
        myImage.color = buffColor;
        SetDuration(buffStack);
        this.description = description;
    }
    public void SetDuration(int buffStack) { stackDisplay.text = buffStack.ToString(); }
}
