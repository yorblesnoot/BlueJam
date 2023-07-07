using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckViewerUI : MonoBehaviour
{
    [SerializeField] RunData runData;
    [SerializeField] List<CardDisplay> cardDisplays;
    [SerializeField] Toggle removal;
    private void OnEnable()
    {
        foreach (var card in cardDisplays)
        {
            card.gameObject.SetActive(false);
        }
        //activate card displays for each card in the players deck
        for(int c = 0; c < runData.playerDeck.deckContents.Count; c++)
        {
            cardDisplays[c].gameObject.SetActive(true);
            runData.playerDeck.deckContents[c].Initialize();
            cardDisplays[c].PopulateCard(runData.playerDeck.deckContents[c]);
        }
        EventManager.clickedCard.AddListener(RemoveCard);
    }

    private void OnDisable()
    {
        EventManager.clickedCard.RemoveListener(RemoveCard);
    }

    public void RemoveCard(CardPlus card, GameObject cardObject)
    {
        //check if the player has enough removes, the switch is toggled, and deck size is high enough--then remove a card
        if(removal.isOn && runData.playerDeck.deckContents.Count > 5 && runData.RemoveStock > 0)
        {
            runData.RemoveStock--;
            EventManager.updateWorldCounters?.Invoke();
            runData.playerDeck.deckContents.Remove(card);
            cardObject.SetActive(false);
        }
    }
}
