using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHandPlus : HandPlus
{
    [SerializeField] GameObject deckSpot;
    [SerializeField] GameObject handSpot;
    [SerializeField] GameObject discardSpot;
    [SerializeField] GameObject conjureSpot;

    readonly float rotationFactor = 4f;
    readonly int pileDisplacementFactor = 5;
    readonly int cardFlySteps = 50;
    List<CardSlot> cardSlots = new();

    internal override void BuildVisualDeck()
    {
        GenerateHandSlots(thisUnit.HandSize);
        foreach(CardPlus card in deckRecord.deckContents)
        {
            CardDisplay cardDisplay = RenderCard(card);
            deckCards.Add(cardDisplay);
            cardDisplay.transform.SetParent(deckSpot.transform, false);
        }
        deckCards.Shuffle();
        FanPile(deckCards);
    }

    public override ICardDisplay ConjureCard(CardPlus card, Vector3 location, EffectInject.InjectLocation injectLocation)
    {
        CardDisplay cardDisplay = RenderCard(card, location);
        cardDisplay.transform.SetParent(conjureSpot.transform, false);
        switch (injectLocation)
        {
            case (EffectInject.InjectLocation.HAND):
                StartCoroutine(DrawCard(cardDisplay));
                break;
            case (EffectInject.InjectLocation.DISCARD):
                StartCoroutine(AnimateRecyleCard(cardDisplay, discardSpot));
                discardCards.Add(cardDisplay);
                FanPile(discardCards);
                break;
            case (EffectInject.InjectLocation.DECK):
                StartCoroutine(AnimateRecyleCard(cardDisplay, deckSpot));
                deckCards.Add(cardDisplay);
                deckCards.Shuffle();
                FanPile(deckCards);
                break;
        }
        return cardDisplay;
    }

    private void GenerateHandSlots(int handSize)
    {
        RectTransform handRect = handSpot.GetComponent<RectTransform>();
        float width = handRect.rect.width;
        float height = handRect.rect.height;

        Vector3 cardSpace = new(width / handSize, 0, 0);
        Vector3 startPosition = new(-width / 2, 0, 0);
        startPosition += cardSpace / 2;

        Vector3 heightDifference = new(0, height / 12, 0);
        cardSlots = new();

        float centerPoint = (float)(handSize - 1) / 2;

        Vector3 scaleCounterpoint = handSpot.transform.localScale;
        scaleCounterpoint.x = 1 / scaleCounterpoint.x;
        scaleCounterpoint.y = 1 / scaleCounterpoint.y;
        scaleCounterpoint.z = 1 / scaleCounterpoint.z;

        for (int i = 0; i < handSize; i++)
        {
            float centerDifference = centerPoint - i;
            CardSlot slot = new()
            {
                cardPosition = startPosition + cardSpace * i + heightDifference * -Mathf.Abs(centerDifference),
                cardRotation = Quaternion.Euler(0, 0, centerDifference * rotationFactor),
                cardScale = scaleCounterpoint
            };
            cardSlots.Add(slot);
        }
    }

    public void RegenerateHandSlots()
    {
        List<CardSlot> oldSlots = new(cardSlots);
        GenerateHandSlots(thisUnit.HandSize);
        for (int i = 0; i < oldSlots.Count; i++)
        {
            if (oldSlots[i].reference != null)
            {
                cardSlots[i].reference = oldSlots[i].reference;
                StartCoroutine(cardSlots[i].FlipToCardPosition());
            }
        }
    }

    CardDisplay RenderCard(CardPlus card, Vector3 location = default)
    {
        //scale and rotation for cards 
        Quaternion rotate = Quaternion.Euler(0, 0, 0);
        GameObject newCard = Instantiate(blankCard, location, rotate);
        newCard.transform.localScale = new Vector3(cardSize, cardSize, cardSize);
        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay.cardBack.SetActive(true);
        cardDisplay.owner = thisUnit;
        card.Initialize();
        cardDisplay.PopulateCard(card);
        return cardDisplay;
    }

    void FanPile(List<ICardDisplay> cards, int direction = 1)
    {
        for(int i = 0; i < cards.Count;i++)
        {
            Vector3 cardPosition = new(-i * pileDisplacementFactor * direction, i * pileDisplacementFactor, 0);
            cards[i].transform.localPosition = cardPosition;
            cards[i].transform.SetAsLastSibling();
        }        
    }

    public override IEnumerator DrawCard(ICardDisplay display = null)
    {
        CardDisplay drawn;
        if (display == null) drawn = (CardDisplay)deckCards.Last();
        else drawn = (CardDisplay)display;
        drawn.transform.SetParent(handSpot.transform, true);
        drawn.transform.SetAsFirstSibling();

        if(cardSlots.Count != handCards.Count) RegenerateHandSlots();
        CardSlot slot = cardSlots.FirstOrDefault(x => x.reference == null);
        slot.reference = drawn;
        if(display == null) deckCards.TransferItemTo(handCards, drawn);
        else handCards.Add(drawn);
        yield return StartCoroutine(slot.FlipToCardPosition());
        if (deckCards.Count == 0)
        {
            //keep an eye on this for index errors ~~~
            StartCoroutine(RecycleDeck());
        }
    }

    public override IEnumerator DiscardCard(ICardDisplay Idiscarded, bool played)
    {
        CardDisplay discarded = (CardDisplay)Idiscarded;
        discarded.gameObject.GetComponent<EmphasizeCard>().readyEmphasis = false;
        CardSlot slot = cardSlots.FirstOrDefault(x => x.reference == discarded);
        if (Idiscarded.forceConsume == true && played == true)
        {
            discarded.transform.SetParent(conjureSpot.transform, true);
            handCards.Remove(discarded);
            yield return StartCoroutine(slot.FlipToSpot());
            Destroy(discarded.gameObject);
            yield break;
        }
        discarded.transform.SetParent(discardSpot.transform, true);
        handCards.TransferItemTo(discardCards, discarded);
        yield return StartCoroutine(slot.FlipToSpot());
        FanPile(discardCards, -1);
        yield break;
    }

    protected override void RecyleCard(ICardDisplay card)
    { StartCoroutine(AnimateRecyleCard(card, deckSpot)); }

    IEnumerator AnimateRecyleCard(ICardDisplay card, GameObject spot)
    {
        card.transform.SetParent(spot.transform, true);
        card.transform.SetAsFirstSibling();
        Vector3 startPosition = card.transform.localPosition;
        Vector3 startScale = card.transform.localScale;
        for (int i = 0; i < cardFlySteps; i++)
        {
            card.transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, ((float)i) / cardFlySteps);
            card.transform.localScale = Vector3.Lerp(startScale, Vector3.one, ((float)i) / cardFlySteps);
            yield return new WaitForSeconds(.01f);
        }
        if(spot == deckSpot) FanPile(deckCards);
        else if(spot == discardSpot) FanPile(discardCards, -1);

    }

    internal void UpdateHand()
    {
        foreach(ICardDisplay card in handCards)
        {
            card.thisCard.Initialize();
            card.PopulateCard(card.thisCard);
        }
    }
}

class CardSlot
{
    public Vector3 cardPosition;
    public Quaternion cardRotation;
    public Vector3 cardScale;
    public GameObject cardBack;

    public CardDisplay reference;
    readonly static int cardDrawDiscardSteps = 40;

    public IEnumerator FlipToCardPosition()
    {
        Vector3 startPosition = reference.transform.localPosition;
        Vector3 startScale = reference.transform.localScale;
        Quaternion startRotation = reference.transform.rotation;

        for (int i = 0; i < cardDrawDiscardSteps; i++)
        {
            reference.transform.SetLocalPositionAndRotation(Vector3.Lerp(startPosition, cardPosition, ((float)i) / cardDrawDiscardSteps),
                Quaternion.Slerp(startRotation, cardRotation, ((float)i) / cardDrawDiscardSteps));
            reference.transform.localScale = Vector3.Lerp(startScale, cardScale, ((float)i) / cardDrawDiscardSteps);
            if (reference.cardBack.activeSelf && reference.transform.localRotation.eulerAngles.y <= 90) reference.cardBack.SetActive(false);
            yield return new WaitForSeconds(.01f);
        }
        reference.transform.SetLocalPositionAndRotation(cardPosition, cardRotation);
        reference.transform.localScale = cardScale;
        EmphasizeCard emphasis = reference.gameObject.GetComponent<EmphasizeCard>();
        emphasis.originalPosition = cardPosition;
        emphasis.originalScale = cardScale;
        emphasis.readyEmphasis = true;
        //set up animation for card movement
        yield return null;
    }

    public IEnumerator FlipToSpot()
    {
        Transform transform = reference.transform;
        EmphasizeCard emphasis = reference.gameObject.GetComponent<EmphasizeCard>();
        emphasis.readyEmphasis = false;
        Vector3 startPosition = transform.localPosition;
        Vector3 startScale = transform.localScale;
        Quaternion startRotation = transform.localRotation;
        GameObject cardBack = reference.cardBack;
        reference = null;
        for (int i = 0; i < cardDrawDiscardSteps; i++)
        {
            transform.SetLocalPositionAndRotation(Vector3.Lerp(startPosition, Vector3.zero, ((float)i) / cardDrawDiscardSteps),
                Quaternion.Slerp(startRotation, Quaternion.identity, ((float)i) / cardDrawDiscardSteps));
            transform.localScale = Vector3.Lerp(startScale, Vector3.one, ((float)i) / cardDrawDiscardSteps);
            if (!cardBack.activeSelf && transform.localRotation.eulerAngles.y >= 270) cardBack.SetActive(true);
            yield return new WaitForSeconds(.01f);
        }
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        transform.localScale = Vector3.one;

        //set up animation for card movement
        yield return null;
    }
}
