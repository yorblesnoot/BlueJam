using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonplayerHandPlus : HandPlus
{
    private Vector3[] cardSlots;
    private void Awake()
    {
        cardFlyDelay = 0;
    }

    internal override void BuildVisualDeck()
    {
        foreach(CardPlus card in deckRecord.deckContents)
        {
            ICardDisplay cardDisplay = RenderCard(card);
            deckCards.Add(cardDisplay);
        }
        deckCards.Shuffle();
    }

    ICardDisplay RenderCard(CardPlus card)
    {
        //scale and rotation for cards 
        GameObject newCard = Instantiate(blankCard, new Vector3(0, -20, 0), Quaternion.identity);
        newCard.transform.SetParent(handSpot.transform, false);
        newCard.transform.localScale = new Vector3(cardSize, cardSize, cardSize);
        ICardDisplay cardDisplay = newCard.GetComponent<NPCCardDisplay>();
        cardDisplay.owner = thisUnit;
        card.Initialize();
        cardDisplay.PopulateCard(card);
        cardDisplay.gameObject.SetActive(false);
        return cardDisplay;
    }

    public override IEnumerator DrawCard(ICardDisplay drawn = null)
    {          
        drawn ??= deckCards[0];
        drawn.gameObject.SetActive(true);
        deckCards.TransferItemTo(handCards, drawn);
        PositionCards();
        yield break;
    }
    public override ICardDisplay ConjureCard(CardPlus card, Vector3 location, EffectInject.InjectLocation injectLocation)
    {
        ICardDisplay cardDisplay = RenderCard(card);
        switch (injectLocation)
        {
            case (EffectInject.InjectLocation.HAND):
                StartCoroutine(DrawCard(cardDisplay));
                break;
            case (EffectInject.InjectLocation.DISCARD):
                discardCards.Add(cardDisplay);
                discardCards.Shuffle();
                break;
            case (EffectInject.InjectLocation.DECK):
                deckCards.Add(cardDisplay);
                deckCards.Shuffle();
                break;
        }
        return cardDisplay;
    }
    public override IEnumerator DiscardCard(ICardDisplay discarded, bool played)
    {
        if (discarded.forceConsume == true && played == true)
        {
            handCards.Remove(discarded);
            Destroy(discarded.gameObject);
        }
        else
        {
            handCards.TransferItemTo(discardCards, discarded);
            discarded.gameObject.SetActive(false);
        }
        PositionCards();
        yield break;
    }

    void PositionCards()
    {
        StopAllCoroutines();
        //find the length and width of the camera to render cards at a set interval
        RectTransform canvasRect = unitCanvas.GetComponent<RectTransform>();
        float width = canvasRect.rect.width;

        //maths to find the middle of the first card interval

        int handSize = handCards.Count;
        float initialX = width * (handSize - 1) / (2 * handSize) * -1;
        //build an array of places for cards to be
        cardSlots = new Vector3[handSize];
        for (int count = 0; count < cardSlots.Length; count++)
        {
            float cardX;
            cardX = initialX + (count * width / handSize);

            cardSlots[count] = new Vector3(cardX, handSpot.transform.localPosition.y, handSpot.transform.localPosition.z);
        }
        StartCoroutine(ArrangeCards());
    }

    [SerializeField] float cardMoveDuration;
    
    public IEnumerator ArrangeCards()
    {
        for (int card = 0; card < handCards.Count; card++)
        {
            GameObject cardObj = handCards[card].gameObject;
            StartCoroutine(cardObj.LerpTo(cardSlots[card], cardMoveDuration, true));
            yield return new WaitForSeconds(cardMoveDuration/2);
        }
    }
}
