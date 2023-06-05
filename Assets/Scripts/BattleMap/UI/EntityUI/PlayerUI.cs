using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : EntityUI
{

    private Vector3[] cardSlots;
    private void Awake()
    {
        cardSize = 1;
    }

    public override void PositionCards()
    {
        //find the length and width of the camera to render cards at a set interval
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect * 50;

        //distance and height at which to render cards
        RectTransform canvasRect = unitCanvas.GetComponent<RectTransform>();
        float cardHeight = -canvasRect.rect.height/ 2;
        //maths to find the middle of the first card interval
        
        int handSize = myHand.maxSize;
        float initialX = width * (handSize-1) / (2*handSize) * -1;
        //build an array of places for cards to be
        cardSlots = new Vector3[handSize];
        for(int count = 0; count < cardSlots.Length; count++)
        {
            float cardX;
            cardX = initialX + (count * width/handSize);

            cardSlots[count] = new Vector3(cardX, cardHeight, 0);
        }
        for (int i = 0; i < myHand.handObjects.Count; i++)
        {
            GameObject cardObj = myHand.handObjects[i];
            StartCoroutine(ArrangeCards(cardObj, cardSlots[i]));
        }
    }

    public IEnumerator ArrangeCards(GameObject card, Vector3 position)
    {
        EmphasizeCard emphasize = card.GetComponent<EmphasizeCard>();
        emphasize.readyEmphasis = false;
        int stepCount = 50;
        float relocateDelta = 20f;
        
        for(int step = 0; step < stepCount; step++)
        {
            card.transform.localPosition = Vector3.MoveTowards(card.transform.localPosition, position, relocateDelta);
            yield return new WaitForSeconds(.01f);
        }
        emphasize.PrepareForEmphasis();
    }

    public void MullPress()
    {
        if(gameObject.GetComponent<UnitActions>().myTurn == true)
        {
            EventManager.clearActivation?.Invoke();
            myHand.DiscardAll();
        }
    }

    public override void UpdateBeats() { }


}