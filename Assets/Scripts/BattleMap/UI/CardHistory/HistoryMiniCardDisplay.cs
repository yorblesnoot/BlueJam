using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HistoryMiniCardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BattleUnit owner { get; set; }
    public CardPlus thisCard { get; set; }

    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text usesText;
    [SerializeField] Transform cardPosition;

    int uses;

    public void PopulateMiniCard(CardPlus card, BattleUnit owner)
    {
        uses = 1;
        usesText.text = "";
        thisCard = card;
        nameText.text = card.displayName;
        this.owner = owner;
    }

    public void AugmentUseCount()
    {
        uses++;
        usesText.text = $"{uses}x";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CardProjector.ProjectCardFromCanvas(thisCard, owner, transform.TransformPoint(cardPosition.localPosition));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        CardProjector.HideProjectedCard();
    }
    
}
