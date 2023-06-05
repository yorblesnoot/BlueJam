using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using System;

public class CardAwardUI : MonoBehaviour
{
    public List<GameObject> awardCards;
    public RunData runData;
    public EssenceCrafting essenceCrafting;
    private void Awake()
    {
        EventManager.clickedCard.AddListener(FinalizeAward);
    }
    public void AwardCards(List<CardPlus> drops)
    {
        for (int i = 0; i < awardCards.Count; i++)
        {
            if (i < drops.Count)
            {
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
        essenceCrafting.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
