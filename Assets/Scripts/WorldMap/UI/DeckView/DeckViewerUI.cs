using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckViewerUI : MonoBehaviour
{
    [SerializeField] RunData runData;
    [SerializeField] List<PlayerCardDisplay> cardDisplays;
    [SerializeField] Toggle removal;
    [SerializeField] TMP_Text status;

    [SerializeField] Unit player;

    readonly int minimumDeckSize = 9;
    private void OnEnable()
    {
        player.LoadStats();
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
            cardDisplays[c].emphasize.originalScale = cardDisplays[c].transform.localScale;
            cardDisplays[c].emphasize.readyEmphasis = true;
        }
        EventManager.clickedCard.AddListener(RemoveCard);
    }

    private void OnDisable()
    {
        EventManager.clickedCard.RemoveListener(RemoveCard);
    }

    public void BombToggleTutorial()
    {
        Tutorial.CompleteStage(TutorialFor.WORLDDECK, 1);
        Tutorial.EnterStage(TutorialFor.WORLDDECK, 2, "While the icon is red and I have bombs, clicking a card will permanently remove it from my deck. Click X to return to the map.");
    }

    public void RemoveCard(CardPlus card, GameObject cardObject)
    {
        if (!removal.isOn)
        {
            status.text = "Press the button above to toggle card removal.";
            return;
        }
        if(runData.RemoveStock == 0)
        {
            status.text = "Gather bombs in the overworld to remove cards from your deck.";
            return;
        }
        if(runData.playerDeck.deckContents.Count < minimumDeckSize)
        {
            status.text = $"Removing a card would put you below the minimum deck size of {minimumDeckSize}...";
            return;
        }
        SoundManager.PlaySound(SoundType.CARDREMOVED);
        runData.RemoveStock--;
        EventManager.updateWorldCounters?.Invoke();
        runData.playerDeck.deckContents.Remove(card);
        cardObject.SetActive(false);
    }
}
