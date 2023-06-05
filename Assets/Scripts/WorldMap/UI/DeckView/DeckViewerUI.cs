using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckViewerUI : MonoBehaviour
{
    [SerializeField] RunData runData;
    [SerializeField] List<CardDisplay> cardDisplays;
    [SerializeField] Toggle removal;
    private void Awake()
    {
        for(int c = 0; c < runData.playerDeck.deckContents.Count; c++)
        {
            cardDisplays[c].gameObject.SetActive(true);
            runData.playerDeck.deckContents[c].Initialize();
            cardDisplays[c].PopulateCard(runData.playerDeck.deckContents[c]);
        }
        EventManager.clickedCard.AddListener(RemoveCard);
    }

    public void RemoveCard(CardPlus card, GameObject cardObject)
    {
        if(removal.isOn && runData.playerDeck.deckContents.Count > 5 && runData.RemoveStock > 0)
        {
            runData.RemoveStock--;
            runData.playerDeck.deckContents.Remove(card);
            cardObject.SetActive(false);
        }
    }
}
