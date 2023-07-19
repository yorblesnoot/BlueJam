using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandPlus : MonoBehaviour
{
    public GameObject blankCard;
    public Canvas unitCanvas;
    public BattleUnit thisUnit;
    public List<ICardDisplay> handCards = new();
    protected List<ICardDisplay> deckCards = new();
    protected List<ICardDisplay> discardCards = new();
    internal readonly int cardSize = 1;

    public Deck deckRecord;

    protected float cardFlyDelay = 0f;

    public void DrawPhase()
    {
        while (handCards.Count < thisUnit.HandSize)
        {
            if (deckCards.Count == 0)
            {
                if (discardCards.Count == 0) break;
                StartCoroutine(RecycleDeck());
            }
            StartCoroutine(DrawCard());
        }
    }
    internal virtual void BuildVisualDeck() { }

    public void DiscardAll()
    {
        int handCount = handCards.Count;
        for (int i = 0; i < handCount; i++)
        {
            StartCoroutine(DiscardCard(handCards[0], false));
        }
        TurnManager.SpendBeats(thisUnit, 2);
    }
    public virtual IEnumerator RecycleDeck()
    {
        int discards = discardCards.Count;
        discardCards.Shuffle();
        for (int i = 0; i < discards; i++)
        {
            ICardDisplay card = discardCards[0];
            discardCards.TransferItemTo(deckCards, card);
            RecyleCard(card);
            if(cardFlyDelay > 0) yield return new WaitForSeconds(cardFlyDelay);
        }
    }

    protected virtual void RecyleCard(ICardDisplay card)
    { }

    public virtual ICardDisplay ConjureCard(CardPlus card, Vector3 location, EffectInject.InjectLocation injectLocation) { return null; }

    public virtual IEnumerator DrawCard(ICardDisplay display = null) { yield break; }

    public virtual IEnumerator DiscardCard(ICardDisplay Idiscarded, bool played)
    {
        yield break;
    }
}
