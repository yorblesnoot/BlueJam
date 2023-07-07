using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using System;

public class CardAwardUI : MonoBehaviour
{
    [SerializeField] List<GameObject> awardCards;
    [SerializeField] RunData runData;
    [SerializeField] EssenceCrafting essenceCrafting;
    [SerializeField] WorldMenuControl worldMenuControl;
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
        for (int i = 0; i < awardCards.Count; i++)
        {
            if (i < drops.Count)
            {
                awardCards[i].SetActive(true);
                drops[i].Initialize();
                CardDisplay cardDisplay = awardCards[i].GetComponent<CardDisplay>();
                cardDisplay.PopulateCard(drops[i]);
                cardDisplay.thisCard = drops[i];
            }
            else
            {
                awardCards[i].SetActive(false);
            }
        }   
    }

    public void FinalizeAward(CardPlus card, GameObject cardObject)
    {
        runData.playerDeck.deckContents.Add(card);
        for (int i = 0; i < awardCards.Count; i++)
        {
            awardCards[i].SetActive(false);
        }

        //worldMenuControl.ToggleWindow(essenceCrafting.gameObject, true);
        gameObject.SetActive(false);
    }
}
