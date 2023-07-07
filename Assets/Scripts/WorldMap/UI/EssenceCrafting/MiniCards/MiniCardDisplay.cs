using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiniCardDisplay : MonoBehaviour, ICardDisplay, IPointerEnterHandler, IPointerExitHandler
{
    public BattleUnit owner { get; set; }
    public CardPlus thisCard { get; set; }

    [SerializeField] TMP_Text nameText;

    [SerializeField] WrldCardDisplay bigCard;

    public void PopulateCard(CardPlus card)
    {
        thisCard = card;
        nameText.text = card.displayName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ActivateLargeDisplay();
    }

    void ActivateLargeDisplay()
    {
        bigCard.gameObject.SetActive(true);
        thisCard.Initialize();
        bigCard.PopulateCard(thisCard);
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bigCard.gameObject.SetActive(false);
    }
}
