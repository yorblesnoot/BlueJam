using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deck", menuName = "ScriptableObjects/UnitKit/UnitDeck")]
public class Deck : SOWithGUID
{
    public List<CardPlus> deckContents;
    public string preDescription;
    public string symbol;
    public Color32 iconColor;
    [HideInInspector]public string essenceDescription;

    public void Initialize()
    {
        essenceDescription = $"{preDescription}.. Contains: *{string.Join(" *", deckContents)}";
    }
}
