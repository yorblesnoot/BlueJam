using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class NonplayerUI : EntityUI
{
    public Slider sliderBeats;
    public Slider sliderGhostBeats;
    private Vector3[] cardSlots;

    //contents of hand
    
    void Awake()
    {
        cardSize = 1;
        unitActions = GetComponentInParent<BattleUnit>();
        TurnManager.updateBeatCounts.AddListener(UpdateBeats);
        EventManager.hideTurnDisplay.AddListener(HideBeatGhost);
    } 
    
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward ,Camera.main.transform.rotation * Vector3.up);
    }

    public virtual void UpdateBeats()
    {
        if (!unitActions.isDead)
        {
            SetBar(unitActions.currentBeats, TurnManager.beatThreshold + 2, sliderGhostBeats, false);
            StartCoroutine(UpdateBar(unitActions.currentBeats, TurnManager.beatThreshold + 2, sliderBeats, false));
        }
        else TurnManager.updateBeatCounts.RemoveListener(UpdateBeats);
    }

    public void ShowBeatGhost(int beats)
    {
        float beatIn = unitActions.turnSpeed * beats;
        SetBar(unitActions.currentBeats + beatIn, TurnManager.beatThreshold + 2, sliderGhostBeats, false);
    }
    public void HideBeatGhost()
    {
        SetBar(unitActions.currentBeats, TurnManager.beatThreshold + 2, sliderGhostBeats, false);
    }

    public override void PositionCards()
    {
        //find the length and width of the camera to render cards at a set interval
        RectTransform canvasRect = GetComponent<RectTransform>();
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
