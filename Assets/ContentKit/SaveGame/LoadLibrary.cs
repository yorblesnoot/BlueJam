using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LoadLibrary", menuName = "ScriptableObjects/Singletons/LoadLibrary")]
public class LoadLibrary : ScriptableObject
{
    [SerializeField] List<CardPlus> cardsPool;
    [SerializeField] ItemPool itemPool;
    public List<Deck> decksPool;

    public Dictionary<string, CardPlus> cards = new();
    public Dictionary<string, Item> items = new();
    public Dictionary<string, Deck> decks = new();

    public void Initialize()
    {
        //foreach (var card in cardsPool) cards.Add(card.Id, card);
        //foreach (var item in itemPool.awardableItemPool) items.Add(item.Id, item);
        //foreach (var deck in decksPool) decks.Add(deck.Id, deck);

        items = itemPool.awardableItemPool.ToDictionary(x => x.Id);
        decks = decksPool.ToDictionary(x => x.Id);
        cards = cardsPool.ToDictionary(x => x.Id);
    }
}
