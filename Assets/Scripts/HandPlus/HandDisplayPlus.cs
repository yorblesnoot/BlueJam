using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDisplayPlus : MonoBehaviour
{
    public GameObject blankCard;

    public BattleUnit thisUnit;

    internal readonly int cardSize = 1;

    public List<ICardDisplay> deckCards = new();
    public List<ICardDisplay> handCards = new();
    public List<ICardDisplay> discardCards = new();
    internal virtual void BuildVisualDeck(int count) { }

    public virtual void DrawCard(CardPlus card) { }

    public virtual void Discard(ICardDisplay Idiscarded) { }

    public void RecycleDeck()
    {
        int discards = discardCards.Count;
        for (int i = 0; i < discards; i++)
        {
            ICardDisplay card = discardCards[0];
            RecycleCard(card);
            deckCards.Add(card);
            discardCards.Remove(card);
        }
    }

    public virtual void RecycleCard(ICardDisplay card) { }
}
