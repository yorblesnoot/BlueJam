using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHandDisplayPlus : HandDisplayPlus
{
    private Vector3[] cardSlots;
    [SerializeField] HandPlus myHand;
    List<ICardDisplay> inactiveCards = new();

    internal override void BuildVisualDeck(int count)
    {
        for (int i = 0; i < count; i++)
        {
            ICardDisplay card = RenderBlankCard();
            inactiveCards.Add(card);
            card.gameObject.SetActive(false);
        }
    }

    public override void VisualConjure(Vector3 location, bool intoDeck = true, CardPlus card = null)
    {
        ICardDisplay cardDisplay = RenderBlankCard();
        if (card != null)
            StartCoroutine(VisualDraw(card, cardDisplay));
        else
        {
            inactiveCards.Add(cardDisplay);
        }
    }

    ICardDisplay RenderBlankCard()
    {
        //scale and rotation for cards 
        GameObject newCard = Instantiate(blankCard, new Vector3(0, -20, 0), Quaternion.identity);
        newCard.transform.SetParent(unitCanvas.transform, false);
        newCard.transform.localScale = new Vector3(cardSize, cardSize, cardSize);
        ICardDisplay cardDisplay = newCard.GetComponent<NPCCardDisplay>();
        cardDisplay.owner = thisUnit;
        cardDisplay.gameObject.SetActive(false);
        return cardDisplay;
    }

    public override IEnumerator VisualDraw(CardPlus card, ICardDisplay drawn = null)
    {
        drawn ??= inactiveCards[0];
        drawn.gameObject.SetActive(true);
        drawn.PopulateCard(card);
        inactiveCards.TransferItemTo(handCards, drawn);
        PositionCards();
        yield break;
    }

    public override IEnumerator VisualDiscard(ICardDisplay discarded)
    {
        handCards.TransferItemTo(inactiveCards, discarded);
        discarded.gameObject.SetActive(false);
        yield break;
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
