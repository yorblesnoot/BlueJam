using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingMiniCardDisplay : MonoBehaviour, ICardDisplay, IPointerEnterHandler, IPointerExitHandler
{
    public BattleUnit owner { get; set; }
    public CardPlus thisCard { get; set; }

    [SerializeField] TMP_Text nameText;

    [HideInInspector] public WrldCardDisplay bigCard;

    public bool forceConsume { get; set; } = false;

    public void PopulateCard(CardPlus card, Unit owner, bool limited = false)
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
        thisCard.InitializeEffects();
        bigCard.PopulateCard(thisCard, owner);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bigCard.gameObject.SetActive(false);
    }
}