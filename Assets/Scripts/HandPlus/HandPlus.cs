using System.Collections;
using System.Collections.Generic;
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
