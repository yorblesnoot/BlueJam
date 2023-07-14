using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHandDisplayPlus : HandDisplayPlus
{
    [SerializeField] GameObject deckSpot;
    [SerializeField] GameObject handSpot;
    [SerializeField] GameObject discardSpot;

    readonly float rotationFactor = 4f;
    readonly int pileDisplacementFactor = 5;
    readonly float cardFlyDelay = .2f;
    readonly int cardFlySteps = 50;

    List<CardSlot> cardSlots = new();
    internal override void BuildVisualDeck(int count)
    {
        GenerateHandSlots(thisUnit.HandSize);
        for (int i = 0; i < count; i++)
        {
            deckCards.Add(RenderBlankCard());
        }
        FanPile(deckCards);
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

    CardDisplay RenderBlankCard()
    {
        //scale and rotation for cards 
        Quaternion rotate = Quaternion.Euler(0, 0, 0);
        GameObject newCard = Instantiate(blankCard, new Vector3(0, 0, 0), rotate);
        newCard.transform.SetParent(deckSpot.transform, false);
        newCard.transform.localScale = new Vector3(cardSize, cardSize, cardSize);
        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay.cardBack.SetActive(true);
        cardDisplay.owner = thisUnit;
        return cardDisplay;
    }

    void FanPile(List<ICardDisplay> cards, int direction = 1)
    {
        for(int i = 0; i < cards.Count;i++)
        {
            Vector3 cardPosition = new(i * pileDisplacementFactor * direction, i * pileDisplacementFactor, 0);
            cards[i].transform.localPosition = cardPosition;
        }        
    }

    public override IEnumerator VisualDraw(CardPlus card)
    {
        CardDisplay drawn = (CardDisplay)deckCards.Last();
        drawn.PopulateCard(card);
        drawn.transform.SetParent(handSpot.transform, true);
        drawn.transform.SetAsFirstSibling();

        if(cardSlots.Count != thisUnit.HandSize) RegenerateHandSlots();
        CardSlot slot = cardSlots.FirstOrDefault(x => x.reference == null);
        slot.reference = drawn;
        deckCards.TransferItemTo(handCards, drawn);
        yield return StartCoroutine(slot.FlipToCardPosition());
        drawn.cardBack.SetActive(false);
        if (deckCards.Count == 0)
        {
            StartCoroutine(AnimateRecycleDeck());
        }
    }

    public override IEnumerator VisualDiscard(ICardDisplay Idiscarded)
    {
        CardDisplay discarded = (CardDisplay)Idiscarded;
        discarded.gameObject.GetComponent<EmphasizeCard>().readyEmphasis = false;
        discarded.transform.SetParent(discardSpot.transform, true);
        CardSlot slot = cardSlots.FirstOrDefault(x => x.reference == discarded);
        handCards.TransferItemTo(discardCards, discarded);
        yield return StartCoroutine(slot.FlipToSpot(discardSpot));
        
        FanPile(discardCards, -1);
        yield break;
    }

    IEnumerator AnimateRecycleDeck()
    {
        int subMoves = 0;
        int discards = discardCards.Count;
        for (int i = 0; i < discards; i++)
        {
            ICardDisplay card = discardCards[0];
            discardCards.TransferItemTo(deckCards, card);
            StartCoroutine(AnimateRecyleCard(card));
            yield return new WaitForSeconds(cardFlyDelay);
        }

        IEnumerator AnimateRecyleCard(ICardDisplay card)
        {
            subMoves++;
            card.transform.SetParent(unitCanvas.transform, true);
            card.transform.SetAsFirstSibling();
            card.transform.localScale = deckSpot.transform.localScale;
            for (int i = 0; i < cardFlySteps; i++)
            {
                card.transform.position = Vector3.Lerp(discardSpot.transform.position, deckSpot.transform.position, ((float)i)/cardFlySteps);
                yield return new WaitForSeconds(.01f);
            }
            //card.transform.localScale = Vector3.one;
            card.transform.SetParent(deckSpot.transform, false);
            card.transform.localScale = Vector3.one;
            FanPile(deckCards);
            subMoves--;
        }
        yield return new WaitUntil(() => subMoves == 0);
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
    readonly static int cardDrawDiscardSteps = 50;

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
            if (reference.transform.localRotation.eulerAngles.y <= 90) reference.cardBack.SetActive(false);
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

    public IEnumerator FlipToSpot(GameObject spot)
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
            if (transform.localRotation.eulerAngles.y >= 270) cardBack.SetActive(true);
            yield return new WaitForSeconds(.01f);
        }
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        transform.localScale = Vector3.one;

        //set up animation for card movement
        yield return null;
    }
}
