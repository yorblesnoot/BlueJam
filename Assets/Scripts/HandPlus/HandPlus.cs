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

    [SerializeField] HandDisplayPlus display;
    [SerializeField] BattleUnit thisUnit;
    void Awake()
    {
        PrepDeck();
    }

    void PrepDeck()
    {
        //initial draw phase and deck build
        deckDrawable.AddRange(deckRecord.deckContents);
        display.BuildVisualDeck(deckDrawable.Count);
    }

    public void DrawPhase()
    {
        while (currentHand.Count < thisUnit.handSize)
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
        display.DrawCard(cardToDraw);
    }
    public void Discard(ICardDisplay toDiscard, bool played)
    {
        display.Discard(toDiscard);
        if (toDiscard.thisCard.consumed == true & played == true)
        {
            //do nothing, ie, the card is burned
        }
        else deckDiscarded.Add(toDiscard.thisCard);
        currentHand.Remove(toDiscard.thisCard);
    }

    public void Discard(CardPlus toDiscard, bool played)
    {
        Debug.Log(toDiscard);
        ICardDisplay toDiscardDisplay = display.handCards.FirstOrDefault(card => card.thisCard == toDiscard);
        Debug.Log(toDiscardDisplay);
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
        display.RecycleDeck();
    }

    public void DiscardAll()
    {
        //implement discard all functionality~~~~~~~~~~~~~~~~~~~~~~~~~~
    }

    public void UpdateHand()
    {
        /*for (int i = 0; i < handReferences.Count; i++)
        {
            handReferences[i].AssembleDescription();
            handObjects[i].GetComponent<CardDisplay>().PopulateCard(handReferences[i]);
        }*/
    }

    public static List<CardPlus> Shuffle(List<CardPlus> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            CardPlus value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
}
