using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Hand : MonoBehaviour
{
    [HideInInspector]
    public int maxSize;

    [HideInInspector]
    public List<GameObject> handObjects;
    public List<CardPlus> handReferences;

    [HideInInspector]
    public string ownerType;

    public Deck deckRecord;

    [HideInInspector] public List<CardPlus> deckDrawable;
    [HideInInspector] public List<CardPlus> deckDiscarded;

    public EntityUI myUI;

    void Awake()
    {
        ownerType = this.gameObject.tag;
        deckDrawable = new List<CardPlus>();
        deckDiscarded = new List<CardPlus>();
        deckDiscarded.AddRange(deckRecord.deckContents);
        handObjects = new List<GameObject>();
        handReferences = new List<CardPlus>();
        TurnManager.turnDraw.AddListener(DrawPhase);
    }

    public void DrawPhase()
    {
        maxSize = GetComponent<UnitActions>().handSize;
        while(handObjects.Count < maxSize)
        {
            if(deckDrawable.Count == 0)
            {
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
        //the SO design of cards is causing an issue bc a single instance of each card is shared among all users

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
        toDiscard.transform.position = new Vector3(-100,-100,-100);
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
        TurnManager.SpendBeats(gameObject, mulliganCost);
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
