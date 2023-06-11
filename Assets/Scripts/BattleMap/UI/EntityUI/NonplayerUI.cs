using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class NonplayerUI : EntityUI
{
    private Vector3[] cardSlots;

    //contents of hand
    
    void Awake()
    {
        cardSize = 1;
        unitActions = GetComponentInParent<BattleUnit>();
        TurnManager.startUpdate.AddListener(UpdateBeats);
    } 
    
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward ,Camera.main.transform.rotation * Vector3.up);
    }

    public override void PositionCards()
    {
        //find the length and width of the camera to render cards at a set interval
        Camera cam = Camera.main;
        RectTransform canvasRect = GetComponent<RectTransform>();
        float height = canvasRect.rect.height;
        float width = canvasRect.rect.width;

        //distance and height at which to render cards
        int cardDistance = -170;
        int cardHeight = 200;

        //maths to find the middle of the first card interval
        
        int handSize = myHand.maxSize;
        float initialX = width * (handSize-1) / (2*handSize) * -1;
        //build an array of places for cards to be
        cardSlots = new Vector3[handSize];
        for(int count = 0; count < cardSlots.Length; count++)
        {
            float cardX;
            cardX = initialX + (count * width/handSize);

            cardSlots[count] = new Vector3(cardX, cardHeight, cardDistance);
        }
        StartCoroutine(ArrangeCards());
    }

    public IEnumerator ArrangeCards()
    {
        
        int stepCount = 50;
        float relocateDelta = 20f;
        
        for(int step = 0; step < stepCount; step++)
        {
            for(int card = 0; card < myHand.handObjects.Count; card++)
            {
                GameObject cardObj = myHand.handObjects[card];
                cardObj.transform.localPosition = Vector3.MoveTowards(cardObj.transform.localPosition, cardSlots[card], relocateDelta);
            }
            yield return new WaitForSeconds(.01f);
        }
        for(int card = 0; card < myHand.handObjects.Count; card++)
        {
            GameObject cardObj = myHand.handObjects[card];
        }
    }  
}
