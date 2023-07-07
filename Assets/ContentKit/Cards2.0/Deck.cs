using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Deck", menuName = "ScriptableObjects/UnitKit/UnitDeck")]
public class Deck : SOWithGUID
{
    public List<CardPlus> deckContents;
    public string symbol;
    public Color32 iconColor;
    [HideInInspector]public string essenceDescription;

    public void Initialize()
    {
        List<string> cardNames = deckContents.Select(x=> x.displayName).ToList();
        string myName = name;
        myName = myName.Replace("Deck", "");
        essenceDescription = $"Essence of the {myName}. Contains:";
    }
}
