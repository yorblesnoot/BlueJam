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
        EventManager.addCard.AddListener(FinalizeAward);
    }
    public void AwardCards(List<CardPlus> drops)
    {
        for (int i = 0; i < awardCards.Count; i++)
        {
            if (i < drops.Count)
            {
                drops[i].Initialize(GameObject.FindGameObjectWithTag("Player"));
                CardDisplay cardDisplay = awardCards[i].GetComponent<CardDisplay>();
                cardDisplay.PopulateCard(drops[i]);
                cardDisplay.thisCard = drops[i];
                EmphasizeCard emphasis = awardCards[i].GetComponent<EmphasizeCard>();
                emphasis.readyEmphasis = true;
                emphasis.positionFactor = 1;
            }
            else
            {
                awardCards[i].SetActive(false);
            }
        }   
    }

    public void FinalizeAward(CardPlus cardObject)
    {
        runData.playerDeck.deckContents.Add(cardObject);
        for (int i = 0; i < awardCards.Count; i++)
        {
            awardCards[i].SetActive(false);
        }
        essenceCrafting.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
