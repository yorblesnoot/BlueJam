using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandPlus : MonoBehaviour
{
    [HideInInspector] public List<CardPlus> deckDrawable = new();
    [HideInInspector] public List<CardPlus> deckDiscarded = new();
    [HideInInspector] public List<CardPlus> currentHand = new();
    public Deck deckRecord;

    public HandDisplayPlus display;
    [SerializeField] BattleUnit thisUnit;

    public void PrepDeck()
    {
        //initial draw phase and deck build
        deckDrawable.AddRange(deckRecord.deckContents);
        deckDrawable = Shuffle(deckDrawable);
        display.BuildVisualDeck(deckDrawable.Count);
    }

    public void DrawPhase()
    {
        while (currentHand.Count < thisUnit.HandSize)
        {
            if (deckDrawable.Count == 0)
            {
                if (deckDiscarded.Count == 0) return;
                RecycleDeck();
            }
            DrawCard(FromTopOfDeck());
        }
    }

    public void DrawCard(CardPlus cardToDraw)
    {
        //draw a card into a UI based on hand owner
        //rendered cards send play instructions
        cardToDraw.Initialize();

        //tell the UI to pull a card from the visual deck~~~~
        currentHand.Add(cardToDraw);
        StartCoroutine(display.VisualDraw(cardToDraw));
    }
    public void Discard(ICardDisplay toDiscard, bool played)
    {
        StartCoroutine(display.VisualDiscard(toDiscard));
        if (toDiscard.thisCard.consumed == true & played == true)
        {
            //do nothing, ie, the card is burned
        }
        else deckDiscarded.Add(toDiscard.thisCard);
        currentHand.Remove(toDiscard.thisCard);
    }

    public void Discard(CardPlus toDiscard, bool played)
    {
        ICardDisplay toDiscardDisplay = display.handCards.FirstOrDefault(card => card.thisCard == toDiscard);
        Discard(toDiscardDisplay, played);
    }

    public CardPlus FromTopOfDeck()
    {
        CardPlus output = deckDrawable[0];
        deckDrawable.RemoveAt(0);
        return output;
    }

    public void RecycleDeck()
    {
        deckDrawable.AddRange(deckDiscarded);
        deckDiscarded = new();
        deckDrawable = Shuffle(deckDrawable);
    }

    public void DiscardAll()
    {
        int handCount = currentHand.Count;
        for (int i = 0; i < handCount; i++)
        {
            Discard(currentHand[0], false);
        }
        TurnManager.SpendBeats(thisUnit, 2);
    }

    public static List<CardPlus> Shuffle(List<CardPlus> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
        return list;
    }
}
