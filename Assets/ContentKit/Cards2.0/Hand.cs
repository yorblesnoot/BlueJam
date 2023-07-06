using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Hand : MonoBehaviour
{
    [HideInInspector]
    public int maxSize;

    [HideInInspector]
    public List<GameObject> handObjects = new();
    public List<CardPlus> handReferences = new();

    [HideInInspector]
    public string ownerType;

    public Deck deckRecord;

    [HideInInspector] public List<CardPlus> deckDrawable = new();
    [HideInInspector] public List<CardPlus> deckDiscarded = new();
    public EntityUI myUI;

    void Awake()
    {
        ownerType = gameObject.tag;
        deckDiscarded.AddRange(deckRecord.deckContents);
        TurnManager.drawThenBuffPhase.AddListener(DrawPhase);
    }

    public void DrawPhase()
    {
        maxSize = GetComponent<BattleUnit>().handSize;
        while(handObjects.Count < maxSize)
        {
            if(deckDrawable.Count == 0)
            {
                if (deckDiscarded.Count == 0) return;
                CycleDeck();
            }
            DrawCard(FromTopOfDeck());
        }
    }

    public void CycleDeck()
    {
        deckDrawable.AddRange(deckDiscarded);
        deckDiscarded = new List<CardPlus>();
        deckDrawable = Shuffle(deckDrawable);
    }

    public CardPlus FromTopOfDeck()
    {
        CardPlus output = deckDrawable[0];
        deckDrawable.RemoveAt(0);
        return output;
    }

    public void DrawCard(CardPlus cardToDraw)
    {
        //draw a card into a UI based on hand owner
        //rendered cards send play instructions
        cardToDraw.Initialize();
        GameObject newCard = myUI.RenderBlank(cardToDraw);
        handObjects.Add(newCard);
        handReferences.Add(cardToDraw);
        if(handObjects.Count == maxSize)
        {
            myUI.PositionCards();
        }
    }

    public void Discard(GameObject toDiscard, bool played)
    {
        //send the card to the shadow realm, then destroy it later
        toDiscard.transform.position = new Vector3(-1000,-1000,-1000);
        Destroy(toDiscard, 5f);
        int discardIndex = handObjects.IndexOf(toDiscard);
        
        if (handReferences[discardIndex].consumed == true & played == true)
        {
            //do nothing, ie, the card is burned
        }
        else deckDiscarded.Add(handReferences[discardIndex]);

        handObjects.RemoveAt(discardIndex);
        handReferences.RemoveAt(discardIndex);
    }

    public void Discard(CardPlus toDiscard, bool played)
    {
        Discard(handObjects[handReferences.IndexOf(toDiscard)], played);
    }

    public void DiscardAll()
    {
        int mulliganCost = 2;
        int handTotal = handObjects.Count;
        for(int count = 0; count < handTotal; count++)
        {
            Discard(handObjects[0], false);
        }
        TurnManager.SpendBeats(gameObject.GetComponent<BattleUnit>(), mulliganCost);
    }

    public void UpdateHand()
    {
        for (int i = 0; i < handReferences.Count; i++)
        {
            handReferences[i].AssembleDescription();
            handObjects[i].GetComponent<CardDisplay>().PopulateCard(handReferences[i]);
        }
    }

    public static List<CardPlus> Shuffle(List<CardPlus> list)  
    {  
        int n = list.Count;  
        while (n > 1) 
        {  
            n--;  
            int k = Random.Range(0, n + 1);  
            CardPlus value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }
        return list;  
    }
}
