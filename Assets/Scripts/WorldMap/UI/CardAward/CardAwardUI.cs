using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using System;

public class CardAwardUI : MonoBehaviour
{
    [SerializeField] List<PlayerCardDisplay> awardCards;
    [SerializeField] RunData runData;
    [SerializeField] EssenceCrafting essenceCrafting;
    private void OnEnable()
    {
        EventManager.clickedCard.AddListener(FinalizeAward);
    }
    private void OnDisable()
    {
        EventManager.clickedCard.RemoveListener(FinalizeAward);
    }
    public void AwardCards(List<CardPlus> drops)
    {
        WorldPlayerControl.playerState = WorldPlayerState.SELECTION;
        for (int i = 0; i < awardCards.Count; i++)
        {
            if (i < drops.Count)
            {
                awardCards[i].gameObject.SetActive(true);
                drops[i].Initialize();
                awardCards[i].thisCard = drops[i];
                awardCards[i].PopulateCard(drops[i]);
                EmphasizeCard emphasis = awardCards[i].GetComponent<EmphasizeCard>();
                emphasis.originalScale = awardCards[i].transform.localScale;
                emphasis.readyEmphasis = true;
            }
            else
            {
                awardCards[i].gameObject.SetActive(false);
            }
        }   
    }

    public void FinalizeAward(CardPlus card, GameObject cardObject)
    {
        runData.playerDeck.deckContents.Add(card);
        new SaveContainer(runData).SaveGame();
        Tutorial.CompleteStage(TutorialFor.WORLDCRAFTING, 2);
        Tutorial.EnterStage(TutorialFor.WORLDCRAFTING, 3, "Nice! When you're done crafting, click the X in the top right or hit ESC, then click the scroll in the top right or press D to see my current deck.");
        gameObject.SetActive(false);
        WorldPlayerControl.playerState = WorldPlayerState.MENUS;
    }
}
