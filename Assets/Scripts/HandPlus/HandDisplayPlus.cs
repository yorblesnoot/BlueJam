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
    internal virtual void BuildVisualDeck(int count)
    { }

    public virtual void DrawCard(CardPlus card) { }

    public virtual void Discard(ICardDisplay Idiscarded) { }

    public virtual void RecycleDeck() { }
}
