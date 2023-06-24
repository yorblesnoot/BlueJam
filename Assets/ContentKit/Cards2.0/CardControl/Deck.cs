using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deck", menuName = "ScriptableObjects/UnitDeck")]
public class Deck : SOWithGUID
{
    public List<CardPlus> deckContents;
    public string preDescription;
    public string symbol;
    [HideInInspector]public string essenceDescription;

    public void Initialize()
    {
        essenceDescription = $"{preDescription}.. Contains: *{string.Join(" *", deckContents)}";
    }
}
