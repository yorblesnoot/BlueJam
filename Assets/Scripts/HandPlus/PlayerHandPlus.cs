using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHandPlus : HandPlus
{
    [SerializeField] GameObject deckSpot;
    [SerializeField] GameObject handSpot;
    [SerializeField] GameObject discardSpot;
    [SerializeField] GameObject conjureSpot;

    [SerializeField] CardPlus hesitationCurse;

    
    readonly float cardFlyTime = .2f;
    List<CardSlot> cardSlots = new();
    private void Awake()
    {
        cardFlyDelay = .1f;
    }

    private KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
    };

    private void Update()
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            KeyCode code = keyCodes[i];
            if (Input.GetKeyDown(code) && i < handCards.Count)
            {
                BtlCardDisplay card = (BtlCardDisplay)cardSlots[i].reference;
                card.ActivateCard();
            }                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
        }
    }
    internal override void BuildVisualDeck()
    {
        GenerateHandSlots(Mathf.RoundToInt(thisUnit.loadedStats[StatType.HANDSIZE]));
        foreach(CardPlus card in deckRecord.deckContents)
        {
            AddVisualCard(card);
        }

        for(int i = 0; i < Settings.Balance.HesitationCurses; i++)
        {
            AddVisualCard(hesitationCurse);
        }

        deckCards.Shuffle();
        FanPile(deckCards);
    }

    void AddVisualCard(CardPlus card)
    {
        PlayerCardDisplay cardDisplay = RenderCard(card);
        deckCards.Add(cardDisplay);
        cardDisplay.transform.SetParent(deckSpot.transform, false);
    }

    public override ICardDisplay ConjureCard(CardPlus card, Vector3 location, EffectInject.InjectLocation injectLocation)
    {
        PlayerCardDisplay cardDisplay = RenderCard(card, location);
        cardDisplay.transform.SetParent(conjureSpot.transform, false);
        switch (injectLocation)
        {
            case (EffectInject.InjectLocation.HAND):
                StartCoroutine(DrawCard(cardDisplay));
                break;
            case (EffectInject.InjectLocation.DISCARD):
                discardCards.Add(cardDisplay);
                StartCoroutine(AnimateRecyleCard(cardDisplay, discardSpot));
                break;
            case (EffectInject.InjectLocation.DECK):
                deckCards.Add(cardDisplay);
                deckCards.Shuffle();
                StartCoroutine(AnimateRecyleCard(cardDisplay, deckSpot));
                break;
        }
        return cardDisplay;
    }

    readonly int heightDifferenceFactor = 1000; //12 is good
    readonly float rotationFactor = 0f; //4 is good
    private void GenerateHandSlots(int handSize)
    {
        RectTransform handRect = handSpot.GetComponent<RectTransform>();
        float width = handRect.rect.width;
        float height = handRect.rect.height;

        Vector3 cardSpace = new(width / handSize, 0, 0);
        Vector3 startPosition = new(-width / 2, 0, 0);
        startPosition += cardSpace / 2;

        Vector3 heightDifference = new(0, height / heightDifferenceFactor, 0);
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
        GenerateHandSlots(Mathf.RoundToInt(thisUnit.loadedStats[StatType.HANDSIZE]));
        for (int i = 0; i < oldSlots.Count; i++)
        {
            if (oldSlots[i].reference != null)
            {
                cardSlots[i].reference = oldSlots[i].reference;
                StartCoroutine(cardSlots[i].FlipToCardPosition());
            }
        }
    }

    PlayerCardDisplay RenderCard(CardPlus card, Vector3 location = default)
    {
        Quaternion rotate = Quaternion.Euler(0, 0, 0);
        GameObject newCard = Instantiate(blankCard, location, rotate);
        newCard.transform.localScale = new Vector3(cardSize, cardSize, cardSize);
        PlayerCardDisplay cardDisplay = newCard.GetComponent<PlayerCardDisplay>();
        cardDisplay.cardBack.SetActive(true);
        cardDisplay.owner = thisUnit;
        card.Initialize();
        cardDisplay.PopulateCard(card);
        return cardDisplay;
    }

    readonly int pileDisplacementFactor = 2;
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
        PlayerCardDisplay drawn;
        if (display == null) drawn = (PlayerCardDisplay)deckCards.Last();
        else drawn = (PlayerCardDisplay)display;
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
            StartCoroutine(RecycleDeck());
        }
    }

    public override IEnumerator DiscardCard(ICardDisplay Idiscarded, bool played)
    {
        PlayerCardDisplay discarded = (PlayerCardDisplay)Idiscarded;
        discarded.gameObject.GetComponent<EmphasizeCard>().readyEmphasis = false;
        CardSlot slot = cardSlots.FirstOrDefault(x => x.reference == discarded);
        if (Idiscarded.forceConsume == true && played == true)
        {
            discarded.transform.SetParent(conjureSpot.transform, true);
            handCards.Remove(discarded);
            yield return StartCoroutine(slot.FlipToSpot());
            discarded.gameObject.SetActive(false);
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
        SoundManager.PlaySound(SoundType.CARDDEALT);
        card.transform.SetParent(spot.transform, true);
        card.transform.SetAsFirstSibling();
        Vector3 startPosition = card.transform.localPosition;
        Vector3 startScale = card.transform.localScale;
        float timeElapsed = 0;
        while (timeElapsed < cardFlyTime)
        {
            card.transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, timeElapsed / cardFlyTime);
            card.transform.localScale = Vector3.Lerp(startScale, Vector3.one, timeElapsed / cardFlyTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        if(spot == deckSpot) FanPile(deckCards);
        else if(spot == discardSpot) FanPile(discardCards, -1);

    }

    internal void UpdateHand()
    {
        List<ICardDisplay> cards = new();
        cards.AddRange(deckCards);
        cards.AddRange(discardCards);
        cards.AddRange(handCards);
        foreach(ICardDisplay card in cards)
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

    public PlayerCardDisplay reference;
    readonly static float cardDrawDiscardTime = .2f;

    public IEnumerator FlipToCardPosition()
    {
        SoundManager.PlaySound(SoundType.CARDDEALT);
        Vector3 startPosition = reference.transform.localPosition;
        Vector3 startScale = reference.transform.localScale;
        Quaternion startRotation = reference.transform.rotation;

        float timeElapsed = 0;
        while(timeElapsed < cardDrawDiscardTime)
        {
            reference.transform.SetLocalPositionAndRotation(Vector3.Lerp(startPosition, cardPosition, timeElapsed/cardDrawDiscardTime),
                Quaternion.Slerp(startRotation, cardRotation, timeElapsed / cardDrawDiscardTime));
            reference.transform.localScale = Vector3.Lerp(startScale, cardScale, timeElapsed / cardDrawDiscardTime);
            if (reference.cardBack.activeSelf && reference.transform.localRotation.eulerAngles.y <= 90) reference.cardBack.SetActive(false);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        reference.transform.SetLocalPositionAndRotation(cardPosition, cardRotation);
        reference.transform.localScale = cardScale;
        EmphasizeCard emphasis = reference.gameObject.GetComponent<EmphasizeCard>();
        emphasis.originalPosition = cardPosition;
        emphasis.originalScale = cardScale;
        reference.cardBack.SetActive(false);
        emphasis.readyEmphasis = true;
    }

    public IEnumerator FlipToSpot()
    {
        SoundManager.PlaySound(SoundType.CARDDEALT);
        Transform transform = reference.transform;
        EmphasizeCard emphasis = reference.gameObject.GetComponent<EmphasizeCard>();
        emphasis.readyEmphasis = false;
        Vector3 startPosition = transform.localPosition;
        Vector3 startScale = transform.localScale;
        Quaternion startRotation = transform.localRotation;
        GameObject cardBack = reference.cardBack;
        reference = null;
        float timeElapsed = 0;
        while (timeElapsed < cardDrawDiscardTime)
        {
            transform.SetLocalPositionAndRotation(Vector3.Lerp(startPosition, Vector3.zero, timeElapsed / cardDrawDiscardTime),
                Quaternion.Slerp(startRotation, Quaternion.identity, timeElapsed / cardDrawDiscardTime));
            transform.localScale = Vector3.Lerp(startScale, Vector3.one, timeElapsed / cardDrawDiscardTime);
            if (!cardBack.activeSelf && transform.localRotation.eulerAngles.y >= 270) cardBack.SetActive(true);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        transform.localScale = Vector3.one;

        //set up animation for card movement
        yield return null;
    }
}
