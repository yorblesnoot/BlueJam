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
    [HideInInspector] public string essenceDescription;
    [HideInInspector] public string unitName;

    private void Reset()
    {
        iconColor = Color.white;
    }
    public void Initialize()
    {
        List<string> cardNames = deckContents.Select(x=> x.displayName).ToList();
        unitName = name;
        unitName = unitName.Replace("Deck", "");
        essenceDescription = $"Essence of the {unitName}. Contains:";
    }
}
