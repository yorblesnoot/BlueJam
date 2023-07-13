using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandDisplayPlus : MonoBehaviour
{
    [SerializeField] GameObject deckSlot;
    [SerializeField] GameObject discardSlot;

    [SerializeField] GameObject blankCard;

    [SerializeField] BattleUnit thisUnit;

    public List<CardDisplay> cards = new();


    readonly int cardSize = 1;
    internal void BuildVisualDeck(int count)
    {
        for(int i = 0; i < count; i++)
        {
            RenderBlankCard();
        }
    }

    public void RenderBlankCard()
    {
        //scale and rotation for cards 
        Quaternion rotate = Quaternion.Euler(0, 0, 0);
        GameObject newCard = Instantiate(blankCard, new Vector3(0, -20, 0), rotate);
        newCard.transform.SetParent(deckSlot.transform, false);
        newCard.transform.localScale = new Vector3(cardSize, cardSize, cardSize);
        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        //cardDisplay.cardBack.SetActive(true);
        cardDisplay.owner = thisUnit;
        cards.Add(cardDisplay);
        //cardDisplay.PopulateCard(card);
    }

    public void DrawCard(CardPlus card)
    {

    }
}
