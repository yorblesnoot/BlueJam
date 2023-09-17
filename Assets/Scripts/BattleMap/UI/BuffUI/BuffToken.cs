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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }

    public void RenderBuff(EffectRecurring buff, BattleUnit owner)
    {
        description = buff.turnLapseEffect.GenerateDescription(owner).FirstToUpper() + " after each action";
        myImage.sprite = buff.iconImage;
        myImage.color = buff.iconColor;
        SetDuration(buff.duration);
    }

    public void RenderStat(EffectStat stat, BattleUnit owner)
    {
        description = stat.GetSubDescription(owner).FirstToUpper();
        myImage.sprite = stat.iconImage;
        myImage.color = stat.iconColor;
        SetDuration(stat.duration);
    }
    public void SetDuration(int buffStack) 
    {
        tooltipText.text = description + $" for {buffStack} more action{(buffStack == 1 ? "" : "s")}.";
        stackDisplay.text = buffStack.ToString(); 
    }
}
