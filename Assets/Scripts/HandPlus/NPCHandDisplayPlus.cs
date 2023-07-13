using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHandDisplayPlus : HandDisplayPlus
{
    private Vector3[] cardSlots;
    [SerializeField] HandPlus myHand;

    [SerializeField] Canvas unitCanvas;

    internal override void BuildVisualDeck(int count)
    {
        for (int i = 0; i < count; i++)
        {
            NPCCardDisplay card = RenderBlankCard();
            deckCards.Add(card);
            card.gameObject.SetActive(false);
        }
    }

    NPCCardDisplay RenderBlankCard()
    {
        //scale and rotation for cards 
        GameObject newCard = Instantiate(blankCard, new Vector3(0, -20, 0), Quaternion.identity);
        newCard.transform.SetParent(unitCanvas.transform, false);
        newCard.transform.localScale = new Vector3(cardSize, cardSize, cardSize);
        NPCCardDisplay cardDisplay = newCard.GetComponent<NPCCardDisplay>();
        cardDisplay.owner = thisUnit;
        cardDisplay.gameObject.SetActive(false);
        return cardDisplay;
    }

    public override void DrawCard(CardPlus card)
    {
        ICardDisplay drawn = deckCards[0];
        drawn.gameObject.SetActive(true);
        drawn.PopulateCard(card);
        handCards.Add(drawn);
        deckCards.Remove(drawn);
        Debug.Log("draw card to " + handCards.Count);
        PositionCards();
    }

    public override void Discard(ICardDisplay discarded)
    {
        Debug.Log(discarded);
        discardCards.Add(discarded);
        handCards.Remove(discarded);
        Debug.Log("discard card to " + handCards.Count);
        discarded.gameObject.SetActive(false);
        
    }

    public override void RecycleDeck()
    {
        foreach (var card in discardCards)
        {
            deckCards.Add(card);
            discardCards.Remove(card);
        }
    }
    void PositionCards()
    {
        //find the length and width of the camera to render cards at a set interval
        RectTransform canvasRect = unitCanvas.GetComponent<RectTransform>();
        float width = canvasRect.rect.width;

        //distance and height at which to render cards
        int cardDistance = -170;
        int cardHeight = 200;

        //maths to find the middle of the first card interval

        int handSize = handCards.Count;
        float initialX = width * (handSize - 1) / (2 * handSize) * -1;
        //build an array of places for cards to be
        cardSlots = new Vector3[handSize];
        for (int count = 0; count < cardSlots.Length; count++)
        {
            float cardX;
            cardX = initialX + (count * width / handSize);

            cardSlots[count] = new Vector3(cardX, cardHeight, cardDistance);
        }
        StartCoroutine(ArrangeCards());
    }

    public IEnumerator ArrangeCards()
    {

        int stepCount = 50;
        float relocateDelta = 20f;

        for (int step = 0; step < stepCount; step++)
        {
            for (int card = 0; card < handCards.Count; card++)
            {
                GameObject cardObj = handCards[card].gameObject;
                cardObj.transform.localPosition = Vector3.MoveTowards(cardObj.transform.localPosition, cardSlots[card], relocateDelta);
            }
            yield return new WaitForSeconds(.01f);
        }
    }
}
