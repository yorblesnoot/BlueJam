using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckViewerUI : MonoBehaviour
{
    [SerializeField] RunData runData;
    [SerializeField] List<CardDisplay> cardDisplays;
    private void Awake()
    {
        for(int c = 0; c < runData.playerDeck.deckContents.Count; c++)
        {
            cardDisplays[c].gameObject.SetActive(true);
            cardDisplays[c].mode = CardDisplay.CardMode.REMOVE;
            runData.playerDeck.deckContents[c].Initialize();
            cardDisplays[c].PopulateCard(runData.playerDeck.deckContents[c]);
        }
    }
}
