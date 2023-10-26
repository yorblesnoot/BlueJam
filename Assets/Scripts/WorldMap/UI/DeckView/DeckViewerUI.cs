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
    [SerializeField] Animator statusAnimator;

    [SerializeField] Unit player;
    private void OnEnable()
    {
        Tutorial.CompleteStage(TutorialFor.WORLDCRAFTING, 4, true);
        Tutorial.Initiate(TutorialFor.WORLDDECK, TutorialFor.WORLDCRAFTING);
        Tutorial.EnterStage(TutorialFor.WORLDDECK, 1,
            "This is my current deck, which I draw cards from in battle. Click the button on the left to toggle card removal.");
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
        Tutorial.CompleteStage(TutorialFor.WORLDDECK, 2, true);
    }

    public void BombToggleTutorial()
    {
        Tutorial.CompleteStage(TutorialFor.WORLDDECK, 1);
        Tutorial.EnterStage(TutorialFor.WORLDDECK, 2, "While the icon is red and I have bombs, clicking a card will <color=red>permanently remove it</color> from my deck. Click the X or hit ESC to return to the map.");
    }

    readonly static string redFlash = "FlashRed";
    public void RemoveCard(CardPlus card, GameObject cardObject)
    {
        if (!removal.isOn)
        {        
            status.text = "Press the button on the left to toggle card removal.";
            statusAnimator.Play(redFlash);
            return;
        }
        else if (runData.RemoveStock == 0)
        {
            status.text = "Gather bombs in the overworld to remove cards from your deck.";
            statusAnimator.Play(redFlash);
            return;
        }
        else if (runData.playerDeck.deckContents.Count <= Settings.Balance.MinimumDeckSize)
        {
            status.text = $"Removing a card would put you below the minimum deck size of {Settings.Balance.MinimumDeckSize}...";
            statusAnimator.Play(redFlash);
            return;
        }
        else status.text = "";
        SoundManager.PlaySound(SoundType.CARDREMOVED);
        runData.RemoveStock--;
        EventManager.updateWorldCounters?.Invoke();
        runData.playerDeck.deckContents.Remove(card);
        StartCoroutine(RemoveWithAnimation(cardObject));
    }

    IEnumerator RemoveWithAnimation(GameObject card)
    {
        WrldCardDisplay display = card.GetComponent<WrldCardDisplay>();
        yield return StartCoroutine(display.FadeAllComponents());
        card.SetActive(false);
        display.ResetColors();
    }
}
