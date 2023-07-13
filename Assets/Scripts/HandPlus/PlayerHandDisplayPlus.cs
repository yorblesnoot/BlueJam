using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class PlayerHandDisplayPlus : HandDisplayPlus
{
    [SerializeField] GameObject deckSlot;
    [SerializeField] GameObject handSlot;
    [SerializeField] GameObject discardSlot;
    internal override void BuildVisualDeck(int count)
    {
        for(int i = 0; i < count; i++)
        {
            deckCards.Add(RenderBlankCard());
        }
    }

    CardDisplay RenderBlankCard()
    {
        //scale and rotation for cards 
        Quaternion rotate = Quaternion.Euler(0, 0, 0);
        GameObject newCard = Instantiate(blankCard, new Vector3(0, -20, 0), rotate);
        newCard.transform.SetParent(deckSlot.transform, false);
        newCard.transform.localScale = new Vector3(cardSize, cardSize, cardSize);
        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay.cardBack.SetActive(true);
        cardDisplay.owner = thisUnit;
        return cardDisplay;
    }

    public override void DrawCard(CardPlus card)
    {
        CardDisplay drawn = (CardDisplay)deckCards[0];
        drawn.PopulateCard(card);
        drawn.transform.SetParent(handSlot.transform, false);
        drawn.cardBack.SetActive(false);

        handCards.Add(drawn);
        deckCards.Remove(drawn);
    }

    public override void Discard(ICardDisplay Idiscarded)
    {
        CardDisplay discarded = (CardDisplay)Idiscarded;
        discarded.transform.SetParent(discardSlot.transform, false);
        discarded.cardBack.SetActive(true);

        discardCards.Add(discarded);
        handCards.Remove(discarded);
    }

    public override void RecycleDeck()
    {
        foreach(var card in discardCards)
        {
            card.transform.SetParent(deckSlot.transform, false);

            deckCards.Add(card);
            discardCards.Remove(card);
        }
    }


    /*public void PositionCards()
    {
        //find the length and width of the camera to render cards at a set interval
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect * 50;

        //distance and height at which to render cards
        RectTransform canvasRect = unitCanvas.GetComponent<RectTransform>();
        float cardHeight = -canvasRect.rect.height / 2;
        //maths to find the middle of the first card interval

        int handSize = myHand.maxSize;
        float initialX = width * (handSize - 1) / (2 * handSize) * -1;
        //build an array of places for cards to be
        cardSlots = new Vector3[handSize];
        for (int count = 0; count < cardSlots.Length; count++)
        {
            float cardX;
            cardX = initialX + (count * width / handSize);

            cardSlots[count] = new Vector3(cardX, cardHeight, 0);
        }
        for (int i = 0; i < myHand.handObjects.Count; i++)
        {
            GameObject cardObj = myHand.handObjects[i];
            StartCoroutine(ArrangeCards(cardObj, cardSlots[i]));
        }
    }*/
}
