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
        GameObject newCard = Instantiate(blankCard, new Vector3(0, 0, 0), rotate);
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

    public override void RecycleCard(ICardDisplay card)
    {
        card.transform.SetParent(deckSlot.transform, false);
    }


    public void PositionCards()
    {
        RectTransform handRect = handSlot.GetComponent<RectTransform>();
        float width = handRect.rect.width;

        int handSize = handCards.Count;

        Vector3 startPosition = new(-width / 2, 0, 0);

        Vector3 cardSpace = new(width / handSize, 0, 0);

        List<CardSlot> cardSlots = new();

        float centerPoint = (float)(handSize-1) / 2;

        for(int i = 0; i < handSize; i++)
        {
            CardSlot slot = new()
            {
                position = startPosition + cardSpace * i,
                rotation = Quaternion.Euler(0, 0, centerPoint - i),
                reference = handCards[i]
            };
            cardSlots.Add(slot);
        }
    }
}

class CardSlot
{
    public Vector3 position;
    public Quaternion rotation;
    public ICardDisplay reference;

    IEnumerator FlipToPosition()
    {
        //set up animation for card movement
        yield return null;
    }
}
